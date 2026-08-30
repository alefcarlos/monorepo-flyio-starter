using System.Security.Claims;
using AwesomeAssertions;
using Flyio.Demo.GraphQLService.Tests.Client;

namespace Flyio.Demo.GraphQLService.Tests;

public class MeQueryTests : IClassFixture<AuthorizedWebApplicationFactory>
{
    private readonly IDemoGraphQLClient _client;

    public MeQueryTests(AuthorizedWebApplicationFactory factory)
    {
        _client = factory.CreateGraphqQLClient();
    }

    [Fact]
    public async Task Reponse_ShouldBeUnauthorized()
    {
        //Arrange
        string[] expectedClaims = [ClaimTypes.Role, "organization"];

        //Act
        var response = await _client.Me.ExecuteAsync(TestContext.Current.CancellationToken);

        //Assert
        response.Data.Should().NotBeNull();
        response.Data.Me.Claims.All(x => expectedClaims.Contains(x.Key)).Should().BeTrue();
    }
}