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
    ;

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithLifetime(ContainerLifetime.Persistent));

var postgresdb = postgres.AddDatabase("Default");

var apiService = builder.AddDotnetProject("apiservice", "../Flyio.Demo.ApiService/Flyio.Demo.ApiService.csproj")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WaitFor(keycloak)
    .WithExplicitStart()
    .WithHttpHealthCheck("/health");

var graphQLService = builder.AddDotnetProject("graphqlservice", "../Flyio.Demo.GraphQLService/Flyio.Demo.GraphQLService.csproj")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithExplicitStart()
    .WithHttpHealthCheck("/health")
    ;

builder.AddDotnetProject("webfrontend", "../Flyio.Demo.Web/Flyio.Demo.Web.csproj")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WithExplicitStart()
    .WaitFor(apiService);

// var todoMigrations = apiService.AddEFMigrations("todos-migrations", "Flyio.Demo.Todos.Infra.TodosDbContext")
//     .WithMigrationsProject<Projects.Flyio_Demo_Todos>();

builder.AddTerraform("terraform", "../../terraform");

builder.Build().Run();
