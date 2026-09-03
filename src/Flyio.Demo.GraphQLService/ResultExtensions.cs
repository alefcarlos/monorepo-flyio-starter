using Ardalis.Result;
using Flyio.Demo.GraphQLService.Types.Errors;
using IResult = Ardalis.Result.IResult;

namespace Flyio.Demo.GraphQLService;

public static class ResultExtensions
{
    public static Result EnsureSuccess(this Result result)
    {
        if (result.IsSuccess)
            return result;

        throw result.ToException();
    }

    public static Ardalis.Result.Result<T> EnsureSuccess<T>(this Ardalis.Result.Result<T> result)
    {
        if (result.IsSuccess)
            return result;

        throw result.ToException();
    }

    private static Exception ToException(this IResult result)
    {
        return result.Status switch
        {
            ResultStatus.Invalid =>
                new GraphQLValidationException(result),

            ResultStatus.NotFound =>
                new GraphQLNotFoundException(result),

            ResultStatus.Conflict =>
                new GraphQLConflictException(result),

            _ => new GraphQLResultErrorException(result)
        };
    }
}