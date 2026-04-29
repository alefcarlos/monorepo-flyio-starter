using Microsoft.AspNetCore.HttpLogging;
using FluentValidation;
using Flyio.Demo.ApiService.Endpoints;
using Flyio.Demo.ApiService.Infra;
using Flyio.Demo.ApiService.UseCases;
using Microsoft.EntityFrameworkCore;
using Flyio.Demo.ApiService;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args)
    .AddWebApiDefaults()
    ;

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(opt => opt.UseInMemoryDatabase("db"));

builder.Services.AddValidation();

builder.Services.AddMediator(options =>
{
    //options.Telemetry.EnableMetrics = true;
    //options.Telemetry.EnableTracing = true;
    options.ServiceLifetime = ServiceLifetime.Scoped;

    // Supply any TYPE from each assembly you want scanned (the generator finds the assembly from the type)
    options.Assemblies =
    [
        typeof(IApiMarker),
    ];
});

//builder.Services.AddOpenTelemetry()
//    .WithMetrics(metrics => metrics.AddMeter(Mediator.Mediator.MeterName))
//    .WithTracing(tracing => tracing.AddSource(Mediator.Mediator.ActivitySourceName));

builder.Services.AddValidatorsFromAssemblies([typeof(IApiMarker).Assembly]);

builder.Services.Configure<HttpLoggingOptions>(options =>
{
    options.RequestHeaders.Add("Authorization");
    options.ResponseHeaders.Add("WWW-Authenticate");
});

builder.Services.AddAuthentication()
    .AddJwtBearerDefaults()
    ;

builder.Services.AddAuthorizationBuilder();

builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

var app = builder.Build();

app.UseHttpLogging();

app.UseProblemDetailsWithDefaults();

app.MapDefaultWebApiEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.MapTodoEndpoints();

await app.RunAsync();
