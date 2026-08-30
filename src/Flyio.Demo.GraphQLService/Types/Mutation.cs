using Flyio.Demo.Todos.Domain;
using Flyio.Demo.Todos.Infra;
using HotChocolate.Authorization;

namespace Flyio.Demo.GraphQLService.Types;

[MutationType]
public static partial class Mutation
{
    [Authorize]
    public static async Task<TodoEntity> AddTodoAsync(ITodosDbContext context, string name, CancellationToken ct)
    {
        var entity = TodoEntity.CreateNew(name);

        context.Todos.Add(entity);
        await context.SaveChangesAsync(ct);
        return entity;
    } 
}