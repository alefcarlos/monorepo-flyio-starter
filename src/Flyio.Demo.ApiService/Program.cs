using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args)
    .AddWebApiDefaults()
    ;

builder.Services.Configure<HttpLoggingOptions>(options =>
{
    options.RequestHeaders.Add("Authorization");
    options.ResponseHeaders.Add("WWW-Authenticate");
});

builder.Services.AddAuthentication()
    .AddJwtBearerDefaults()
    ;

builder.Services.AddAuthorizationBuilder();

var app = builder.Build();

app.UseHttpLogging();

app.UseProblemDetailsWithDefaults();

app.MapDefaultWebApiEndpoints();

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/setting", (IConfiguration configuration, string key) =>
{
    return configuration[key];
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/authenticated-ping", () =>
{
    return "pong";
})
.RequireAuthorization()
;

await app.RunAsync();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
