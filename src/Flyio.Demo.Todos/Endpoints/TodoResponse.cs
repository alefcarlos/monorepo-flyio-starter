using Flyio.Demo.Todos.Domain;

namespace Flyio.Demo.Todos.Endpoints.Responses;

public record TodoResponse(Guid Id, string Name, bool Done, DateTimeOffset CreatedAt)
{
    public static TodoResponse FromEntity(TodoEntity entity)
    {
        return new(entity.Id.Value, entity.Name, entity.Done, entity.CreatedAt);
    }
}