using System.Security.Claims;
using HotChocolate.Authorization;

namespace GettingStarted.Types;

[QueryType]
public static partial class Query
{
    public static Book GetBook()
        => new Book("C# in depth.", new Author("Jon Skeet"));

    [Authorize]
    public static MeResponse Me(ClaimsPrincipal claimsPrincipal)
    {
        var claims = claimsPrincipal.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
        return new MeResponse(claims);
    }
}

public record MeResponse(IEnumerable<KeyValuePair<string, string>> Claims);