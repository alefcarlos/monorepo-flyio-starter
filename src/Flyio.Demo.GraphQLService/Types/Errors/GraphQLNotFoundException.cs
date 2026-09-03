using IResult = Ardalis.Result.IResult;

namespace Flyio.Demo.GraphQLService.Types.Errors;

public sealed class GraphQLNotFoundException : Exception
{
    public GraphQLNotFoundException(IResult result)
        : base(GetMessage(result))
    {
    }

    private static string GetMessage(IResult result)
        => result.Errors.FirstOrDefault()
           ?? "The requested resource was not found.";
}
