using Flyio.Demo.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose");

var username = builder.AddParameter("username", "admin");
var password = builder.AddParameter("password", "admin");

var keycloak = builder
    .AddKeycloak("keycloak", 8080, adminPassword: password)
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

var graphQLService = builder.AddProject<Projects.Flyio_Demo_GraphQLService>("graphqlservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithHttpHealthCheck("/health")
    ;

var todoMigrations = apiService.AddEFMigrations("todos-migrations", "Flyio.Demo.Todos.Infra.TodosDbContext")
    .WithMigrationsProject<Projects.Flyio_Demo_Todos>();

builder.AddProject<Projects.Flyio_Demo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddTerraform("terraform", "../../terraform");

builder.Build().Run();
