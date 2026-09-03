using Ardalis.Result;
using AwesomeAssertions;
using Flyio.Demo.GraphQLService.Types.Errors;

namespace Flyio.Demo.GraphQLService.Tests;

public class ResultExtensionsTests
{
    [Fact]
    public void EnsureSuccess_ShouldReturnSameResult_WhenResultIsSuccessful()
    {
        var result = Result.Success();

        var actual = result.EnsureSuccess();

        actual.Should().BeSameAs(result);
    }

    [Fact]
    public void EnsureSuccess_ShouldReturnSameResult_WhenGenericResultIsSuccessful()
    {
        var result = Result.Success("todo-id");

        var actual = result.EnsureSuccess();

        actual.Should().BeSameAs(result);
    }

    [Fact]
    public void EnsureSuccess_ShouldReturnSameResult_WhenMappingResultIsSuccessful()
    {
        var result = Result.Success("todo-id");

        result
            .EnsureSuccess().Map(a => a.IndexOf('-'))
            .Value
            .Should().Be(4);
    }

    [Theory]
    [InlineData(ResultStatus.Invalid, typeof(GraphQLValidationException))]
    [InlineData(ResultStatus.NotFound, typeof(GraphQLNotFoundException))]
    [InlineData(ResultStatus.Conflict, typeof(GraphQLConflictException))]
    public void EnsureSuccess_ShouldThrowExpectedException(
        ResultStatus status,
        Type expectedExceptionType)
    {
        var result = CreateResult(status);

        var act = () => result.EnsureSuccess();

        act.Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedExceptionType);
    }

    [Fact]
    public void EnsureSuccess_ShouldThrowGenericException_WhenStatusIsNotMapped()
    {
        var result = Result.Error("Something went wrong.");

        var act = () => result.EnsureSuccess();

        act.Should().Throw<GraphQLResultErrorException>();
    }

    [Fact]
    public void GenericEnsureSuccess_ShouldThrowExpectedException_WhenResultIsNotSuccessful()
    {
        var result = Result<string>.NotFound();

        var act = () => result.EnsureSuccess();

        act.Should().Throw<GraphQLNotFoundException>();
    }

    private static Result CreateResult(ResultStatus status)
    {
        return status switch
        {
            ResultStatus.Invalid => Result.Invalid(new ValidationError("Invalid result.")),
            ResultStatus.NotFound => Result.NotFound(),
            ResultStatus.Conflict => Result.Conflict("Conflict result."),
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }


    [Fact]
    public void EnsureSuccess_ShouldPreserveValidationErrors()
    {
        var validationErrors = new[]
        {
            new ValidationError(
                identifier: "Name",
                errorMessage: "Name is required."),

            new ValidationError(
                identifier: "Name",
                errorMessage: "Name must be at least 3 characters.")
        };

        var result = Result.Invalid(validationErrors);

        var act = () => result.EnsureSuccess();

        var exception = act
            .Should()
            .Throw<GraphQLValidationException>()
            .Which;

        exception
            .Errors
            .Should()
            .BeEquivalentTo(validationErrors);
    }

    [Fact]
    public void EnsureSuccess_ShouldPreserveErrors()
    {
        var result = Result.Error("Something went wrong.");

        var act = () => result.EnsureSuccess();

        var exception = act
            .Should()
            .Throw<GraphQLResultErrorException>()
            .Which;

        exception
            .Message
            .Should()
            .Be("Something went wrong.");
    }

    [Fact]
    public void EnsureSuccess_ShouldPreserveConflictErrors()
    {
        var errors = new[]
        {
            "A Todo with this name already exists."
        };

        var result = Result.Conflict(errors);

        var act = () => result.EnsureSuccess();

        var exception = act
            .Should()
            .Throw<GraphQLConflictException>()
            .Which;

        exception
            .Message
            .Should()
            .Be(errors[0]);
    }
}