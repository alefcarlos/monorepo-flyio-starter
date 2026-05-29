using AwesomeAssertions;
using Flyio.Demo.Todos.Contracts;
using Flyio.Demo.Todos.Domain;
using Flyio.Demo.Todos.Infra;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable.NSubstitute;
using NSubstitute;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Flyio.Demo.ApiService.Tests.Endpoints.GetTodo;

public class EndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Configure<AuthenticationOptions>(x => x.SchemeMap["Bearer"].HandlerType = typeof(ViewerAuthenticationHandler));
            });
        });
    }

    [Fact]
    public async Task Reponse_ShouldBeOk_WithEmptyArray()
    {
        //Arrange
        var mock = Substitute.For<ITodosDbContext>();
        var data = new List<TodoEntity>().BuildMockDbSet();

        mock.Todos.Returns(data);

        var client = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<ITodosDbContext>((_) => mock);
                });
            })
            .CreateClient();

        //Act
        var response = await client.GetAsync("/v1/todos", TestContext.Current.CancellationToken);
        var reponseAsString = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be200Ok();
        reponseAsString.Should().Be("[]");
    }

    [Fact]
    public async Task Reponse_ShouldBeOk_WithExpectedValues()
    {
        //Arrange
        var timestamp = new DateTimeOffset(DateOnly.MinValue, TimeOnly.MinValue, TimeSpan.FromHours(-3));
        var mock = Substitute.For<ITodosDbContext>();
        var data = new List<TodoEntity>()
        {
            new() {  Id = new TodoId(Guid.Parse("e06bc7c5-1eaa-40e9-b436-0ea795290305")), Name ="Name", CreatedAt = timestamp },
            new() {  Id = new TodoId(Guid.Parse("e06bc7c5-1eaa-40e9-b436-0ea795290306")), Name ="Nam2", CreatedAt = timestamp },
        }.BuildMockDbSet();

        mock.Todos.Returns(data);

        var client = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<ITodosDbContext>((_) => mock);
                });
            })
            .CreateClient();

        //Act
        var response = await client.GetAsync("/v1/todos", TestContext.Current.CancellationToken);
        var reponseAsString = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be200Ok();
        reponseAsString.Should().Be("""[{"id":"e06bc7c5-1eaa-40e9-b436-0ea795290305","name":"Name","done":false,"createdAt":"0001-01-01T00:00:00-03:00"},{"id":"e06bc7c5-1eaa-40e9-b436-0ea795290306","name":"Nam2","done":false,"createdAt":"0001-01-01T00:00:00-03:00"}]""");
    }

    private class ViewerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ViewerAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, "apiservice:viewer")
            };

            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Bearer");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}