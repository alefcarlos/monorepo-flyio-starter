using System.Security.Claims;
using Flyio.Demo.Todos.Contracts;
using Flyio.Demo.Todos.Domain;
using Flyio.Demo.Todos.Infra;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Flyio.Demo.GraphQLService.Types;

[QueryType]
public static partial class Query
{
    [Authorize]
    public static async Task<TodoEntity?> GetTodoByIdAsync(ITodosDbContext context, Guid id, CancellationToken cancellationToken)
    {
        var todoId = new TodoId(id);

        return await context.Todos.FirstOrDefaultAsync(x => x.Id == todoId, cancellationToken);
    }

    [Authorize]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<TodoEntity> GetTodos(ITodosDbContext context, CancellationToken cancellationToken) => context.Todos;

    [Authorize]
    public static MeResponse Me(ClaimsPrincipal claimsPrincipal)
    {
        var claims = claimsPrincipal.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
        return new MeResponse(claims);
    }
}

public record MeResponse(IEnumerable<KeyValuePair<string, string>> Claims);