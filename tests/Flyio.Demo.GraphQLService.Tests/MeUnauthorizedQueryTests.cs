using AwesomeAssertions;
using Flyio.Demo.GraphQLService.Tests.Client;

namespace Flyio.Demo.GraphQLService.Tests;

public class MeUnauthorizedQueryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly IDemoGraphQLClient _client;

    public MeUnauthorizedQueryTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateGraphqQLClient();
    }

    [Fact]
    public async Task Reponse_ShouldBeUnauthorized()
    {
        //Arrange
        //Act
        var response = await _client.Me.ExecuteAsync(TestContext.Current.CancellationToken);

        //Assert
        response.Errors.Should().ContainSingle();
        response.Errors.First().Code.Should().Be("AUTH_NOT_AUTHENTICATED");
    }
}