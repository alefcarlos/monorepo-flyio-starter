using Flyio.Demo.Web;
using Flyio.Demo.Web.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args)
    .AddDefaults()
    ;

builder.Services.AddUserSessionManagement();

// Add services to the container.
builder.Services.AddRazorComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    
    //https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-10.0#persisting-keys-when-hosting-in-a-docker-container
    //Para ambiente produtivo É NECESSÁRIO CONFIGURAR AS CHAVES PARA SEREM PERSISTIDAS EXTERNAMENTE
    // builder.Services.AddDataProtection()
    //     .PersistKeysToDbContext<SampleDbContext>();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseOutputCache();

app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>();

app.MapDefaultEndpoints();

app.MapGroup("/authentication").MapLoginAndLogout();

app.Run();