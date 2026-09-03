using IResult = Ardalis.Result.IResult;

namespace Flyio.Demo.GraphQLService.Types.Errors;

public sealed class GraphQLConflictException : Exception
{
    public GraphQLConflictException(IResult result)
        : base(GetMessage(result))
    {
    }

    private static string GetMessage(IResult result)
        => result.Errors.FirstOrDefault()
           ?? "The request conflicts with the current state of the resource.";
}
