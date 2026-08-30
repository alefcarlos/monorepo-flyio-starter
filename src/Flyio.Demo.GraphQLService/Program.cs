using Microsoft.AspNetCore.Authentication.JwtBearer;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults();

builder.AddGraphQL()
    .AddAuthorization()
    .AddInstrumentation()
    .ModifyRequestOptions(x => x.IncludeExceptionDetails = builder.Environment.IsDevelopment())
    .AddMutationConventions()
    .AddTypes();

builder.Authentication
    .AddJwtBearerDefaults()
    // .SetDefaultScheme(JwtBearerDefaults.AuthenticationScheme)
    .Schemes
    .AddJwtBearer();

builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddHotChocolateInstrumentation());

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL("/");

app.MapDefaultEndpoints();

app.RunWithGraphQLCommands(args);
