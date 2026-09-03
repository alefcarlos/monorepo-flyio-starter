using AwesomeAssertions;
using Flyio.Demo.GraphQLService.Tests.Client;

namespace Flyio.Demo.GraphQLService.Tests;

[Collection(nameof(AspireCollection))]
public class MutationTests
{
    private readonly IDemoGraphQLClient _client;

    public MutationTests(AspireFixture fixture)
    {
        _client = fixture.Factory.CreateGraphqQLClient();
    }

    [Fact]
    public async Task AddTodo_ShouldBeOk()
    {
        //Arrange

        //Act
        var response = await _client.AddTodo.ExecuteAsync(nameof(AddTodo_ShouldBeOk), TestContext.Current.CancellationToken);

        //Assert
        response.Data.Should().NotBeNull();
        response.Data.AddTodo.TodoResponse.Should().NotBeNull();
        response.Data.AddTodo.TodoResponse.Name.Should().Be(nameof(AddTodo_ShouldBeOk));
        response.Errors.Should().BeEmpty();
        response.Data.AddTodo.Errors.Should().BeNull();
    }

    [Fact]
    public async Task AddTodo_ShouldBeInvalid_WhenNameIsEmpty()
    {
        //Arrange

        //Act
        var response = await _client.AddTodo.ExecuteAsync("", TestContext.Current.CancellationToken);

        //Assert
        response.Data.Should().NotBeNull();
        response.Data.AddTodo.TodoResponse.Should().BeNull();
        response.Errors.Should().BeEmpty();
        response.Data.AddTodo.Errors.Should().ContainSingle();
    }
}