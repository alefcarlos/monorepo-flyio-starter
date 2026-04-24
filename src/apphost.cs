#:sdk Aspire.AppHost.Sdk@13.2.0
#:property UserSecretsId=118ee23f-c5d9-4935-96c9-4991c066cb88

#:project ./Flyio.Demo.Web/Flyio.Demo.Web.csproj
#:project ./Flyio.Demo.ApiService/Flyio.Demo.ApiService.csproj

#:package Aspire.Hosting.Keycloak

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", "admin");
var password = builder.AddParameter("password", "admin");

var keycloak = builder
    .AddKeycloak("keycloak", 8080, adminPassword: password)
    .WithRealmImport("./keycloak/realms")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    ;

var apiService = builder.AddProject<Projects.Flyio_Demo_ApiService>("apiservice")
    .WaitFor(keycloak)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Flyio_Demo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
