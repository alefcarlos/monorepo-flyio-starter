using FluentValidation;
using Flyio.Demo.ApiService;
using Flyio.Demo.Heart;
using Flyio.Demo.SharedKernel;
using Flyio.Demo.Todos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args)
    .AddWebApiDefaults()
    ;

builder.AddTodosModule();
builder.AddHeartModule();

builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>(); // domain events

builder.Services.AddHttpContextAccessor();

builder.Services.AddValidation();

builder.Services.AddMediator(options =>
{
    options.Telemetry.EnableMetrics = true;
    options.Telemetry.EnableTracing = true;
    options.ServiceLifetime = ServiceLifetime.Scoped;

    // Supply any TYPE from each assembly you want scanned (the generator finds the assembly from the type)
    options.Assemblies =
    [
        typeof(IApiMarker),
        typeof(ITodosModule),
        typeof(IHeartModule),
    ];
});

builder.Services.AddOpenTelemetry()
   .WithMetrics(metrics => metrics.AddMeter(Mediator.Mediator.MeterName))
   .WithTracing(tracing => tracing.AddSource(Mediator.Mediator.ActivitySourceName));

builder.Services.AddValidatorsFromAssemblies([typeof(IApiMarker).Assembly, typeof(ITodosModule).Assembly]);

builder.Services.Configure<HttpLoggingOptions>(options =>
{
    options.RequestHeaders.Add("Authorization");
});

builder.Authentication
    .AddJwtBearerDefaults()
    .SetDefaultScheme(JwtBearerDefaults.AuthenticationScheme)
    .Schemes
    .AddJwtBearer();

builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

var app = builder.Build();

app.UseHttpLogging();

app.UseProblemDetailsWithDefaults();

app.MapDefaultWebApiEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.MapTodosEndpoints();
app.MapHeartEndpoints();

app.MapGet("/whoami", (HttpContext context) =>
{
    static string GetAuthorizationScheme(HttpRequest request) =>
        request.Headers.Authorization.First()!.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

    static string GetAccessToken(HttpRequest request) =>
        request.Headers.Authorization.First()!.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];

    var claims = context.User.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
    var scheme = GetAuthorizationScheme(context.Request);
    var accessToken = GetAccessToken(context.Request);

    return new { scheme, claims, accessToken };
}).RequireAuthorization();

await app.RunAsync();