using Ardalis.Result;
using IResult = Ardalis.Result.IResult;

namespace Flyio.Demo.GraphQLService.Types.Errors;

public sealed class GraphQLValidationException : Exception
{
    public GraphQLValidationException(IResult result)
        : base("The request contains one or more validation errors.")
    {
        Errors = [.. result.ValidationErrors];
    }

    public IReadOnlyCollection<ValidationError> Errors { get; }
}
