#pragma warning disable ASPIRERESOURCETRAITS001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Flyio.Demo.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose");

var username = builder.AddParameter("username", "admin");
var password = builder.AddParameter("password", "admin");

var keycloak = builder
    .AddKeycloak("keycloak", 8080, adminPassword: password)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithExplicitStart()
    .WithTags("Infra")
    ;

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithLifetime(ContainerLifetime.Persistent))
    .WithTags("Infra")
    ;

var postgresdb = postgres.AddDatabase("Default");

var apiService = builder.AddDotnetProject("apiservice", "../Flyio.Demo.ApiService/Flyio.Demo.ApiService.csproj")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WaitFor(keycloak)
    .WithExplicitStart()
    .WithHttpHealthCheck("/health")
    .WithTags("Backend")
    ;

var graphQLService = builder.AddDotnetProject("graphqlservice", "../Flyio.Demo.GraphQLService/Flyio.Demo.GraphQLService.csproj")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithExplicitStart()
    .WithHttpHealthCheck("/health")
    .WithTags("Backend")
    ;

builder.AddDotnetProject("webfrontend", "../Flyio.Demo.Web/Flyio.Demo.Web.csproj")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WithExplicitStart()
    .WaitFor(apiService)
    .WithTags("Frontend")
    ;

var todoMigrations = apiService.AddEFMigrations("todos-migrations", "Flyio.Demo.Todos.Infra.TodosDbContext")
    .WithMigrationsProject("../Flyio.Demo.Todos/Flyio.Demo.Todos.csproj")
    .WithTags("Backend")
    ;

builder.AddTerraform("terraform", "../../terraform")
    .WithTags("Infra")
    ;

builder.Build().Run();
