using IResult = Ardalis.Result.IResult;

namespace Flyio.Demo.GraphQLService.Types.Errors;

public sealed class GraphQLResultErrorException : Exception
{
    public GraphQLResultErrorException(IResult result)
        : base(GetMessage(result))
    {
    }

    private static string GetMessage(IResult result)
        => result.Errors.FirstOrDefault()
           ?? "The requested thrown an unhandled error.";
}