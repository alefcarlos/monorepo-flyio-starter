using Flyio.Demo.Module.SharedKernel.Infra;
using Flyio.Demo.SharedKernel;
using Flyio.Demo.Todos;
using Flyio.Demo.Todos.Infra;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults();

builder.AddGraphQL()
    .AddAuthorization()
    .AddInstrumentation()
    .ModifyRequestOptions(x => x.IncludeExceptionDetails = builder.Environment.IsDevelopment())
    .AddMutationConventions()
    .AddFiltering()
    .AddSorting()
    .AddTypes();

builder.Authentication
    .Schemes
    .AddJwtBearer();

builder.Services.AddMediator(options =>
{
    options.Telemetry.EnableMetrics = true;
    options.Telemetry.EnableTracing = true;
    options.GenerateTypesAsInternal = true;
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.Assemblies = [typeof(ITodosModule)];
});

builder.Services.AddOpenTelemetry()
   .WithMetrics(metrics => metrics.AddMeter(Mediator.Mediator.MeterName))
   .WithTracing(tracing =>
    tracing.AddSource(Mediator.Mediator.ActivitySourceName)
    .AddHotChocolateInstrumentation());

builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ITenantGetter, TenantGetter>();

builder.AddTodosModule();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL("/");

app.MapDefaultEndpoints();

app.RunWithGraphQLCommands(args);
