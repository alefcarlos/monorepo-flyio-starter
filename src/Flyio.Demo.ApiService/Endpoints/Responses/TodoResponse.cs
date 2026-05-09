using Flyio.Demo.ApiService.Entities;

namespace Flyio.Demo.ApiService.Endpoints.Responses;

public record TodoResponse(Guid Id, string Name, DateTimeOffset CreatedAt)
{
    public static TodoResponse FromEntity(TodoEntity entity)
    {
        return new(entity.Id.Value, entity.Name, entity.CreatedAt);
    }
}