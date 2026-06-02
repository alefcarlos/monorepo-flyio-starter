#:sdk Aspire.AppHost.Sdk@13.4.0
#:property UserSecretsId=118ee23f-c5d9-4935-96c9-4991c066cb88

#:project ./src/Flyio.Demo.Web/Flyio.Demo.Web.csproj
#:project ./src/Flyio.Demo.ApiService/Flyio.Demo.ApiService.csproj
#:project ./src/Flyio.Demo.Todos/Flyio.Demo.Todos.csproj

#:package Aspire.Hosting.Docker
#:package Aspire.Hosting.EntityFrameworkCore
#:package Aspire.Hosting.Keycloak
#:package Aspire.Hosting.PostgreSQL

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose");

var username = builder.AddParameter("username", "admin");
var password = builder.AddParameter("password", "admin");

var keycloak = builder
    .AddKeycloak("keycloak", 8080, adminPassword: password)
    .WithRealmImport("./keycloak/realms")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    ;

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050).WithLifetime(ContainerLifetime.Persistent));

var postgresdb = postgres.AddDatabase("Default");

var apiService = builder.AddProject<Projects.Flyio_Demo_ApiService>("apiservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WaitFor(keycloak)
    .WithHttpHealthCheck("/health");

var todoMigrations = apiService.AddEFMigrations("todos-migrations", "Flyio.Demo.Todos.Infra.TodosDbContext")
    .WithMigrationsProject<Projects.Flyio_Demo_Todos>();

builder.AddProject<Projects.Flyio_Demo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
