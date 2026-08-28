using FluentValidation;
using Flyio.Demo.ApiService;
using Flyio.Demo.Heart;
using Flyio.Demo.Module.SharedKernel.Infra;
using Flyio.Demo.SharedKernel;
using Flyio.Demo.Todos;
using Flyio.Demo.Todos.Infra;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args)
    .AddWebApiDefaults()
    ;

builder.Services.Configure<ScalarOptions>(options => options.DisableAgent().DisableMcp());
builder.Services.AddOpenApi("v1", options => options.AddScalarTransformers());

builder.AddTodosModule();
builder.AddHeartModule();

builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>(); // domain events
builder.Services.AddSingleton<ITenantGetter, TenantGetter>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddValidation();

builder.Services.AddMediator(options =>
{
    options.Telemetry.EnableMetrics = true;
    options.Telemetry.EnableTracing = true;
    options.GenerateTypesAsInternal = true;
    options.ServiceLifetime = ServiceLifetime.Scoped;
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

builder.Services.AddAuthorization(opt => opt.DefaultPolicy = new AuthorizationPolicyBuilder()
     .TenantWhenRequired()
     .RequireAuthenticatedUser()
     .Build());

builder.Services.AddSingleton<IAuthorizationHandler, TenantRequirementHandler>();
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
