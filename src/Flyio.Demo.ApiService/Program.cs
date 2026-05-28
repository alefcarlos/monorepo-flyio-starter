using System.Runtime.CompilerServices;
using FluentValidation;
using Flyio.Demo.ApiService;
using Flyio.Demo.ApiService.Endpoints;
using Flyio.Demo.ApiService.Infra;
using Flyio.Demo.ApiService.UseCases;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args)
    .AddWebApiDefaults()
    ;

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(
    (provider, opt) => opt.UseNpgsql(provider.GetRequiredService<IConfiguration>().GetConnectionString("Default")));

builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

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

builder.Authentication
    .AddJwtBearerDefaults()
    .SetDefaultScheme(JwtBearerDefaults.AuthenticationScheme)
    .Schemes
    .AddJwtBearer();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("viewer", policy => policy.RequireRole("apiservice:viewer"))
    .AddPolicy("writer", policy => policy.RequireRole("apiservice:writer"))
    ;

builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

var app = builder.Build();

app.UseHttpLogging();

app.UseProblemDetailsWithDefaults();

app.MapDefaultWebApiEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.MapTodoEndpoints();

app.MapGet("/heart-rate", (CancellationToken cancellationToken) =>
{
    static async IAsyncEnumerable<HeartRateRecord> GetDataAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var rate = Random.Shared.Next(69, 120);
            yield return HeartRateRecord.Create(rate);
            await Task.Delay(1_000, cancellationToken);
        }
    }

    return TypedResults.ServerSentEvents(GetDataAsync(cancellationToken), eventType: "heartRate");
});

await app.RunAsync();

record HeartRateRecord(DateTimeOffset Timestamp, int HeartRate)
{
    public static HeartRateRecord Create(int heartRate) => new(DateTimeOffset.Now, heartRate);
}