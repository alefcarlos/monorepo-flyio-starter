using Ardalis.Result;
using Flyio.Demo.Module.SharedKernel.Entities;
using Flyio.Demo.Todos.Contracts;

namespace Flyio.Demo.Todos.Domain;

public class TodoEntity : BaseEntity
{
    public TodoId Id { get; internal set; }
    public string Name { get; internal set; } = string.Empty;

    public bool Done { get; internal set; }

    internal TodoEntity() { }

    public static Result<TodoEntity> CreateNew(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Invalid(new ValidationError(nameof(name), "Name is required"));
        }

        var entity = new TodoEntity
        {
            Id = TodoId.NewTodoId(),
            Name = name
        };

        return entity;
    }

    public Result<TodoEntity> SetDone()
    {
        if (Done)
        {
            return Result.Conflict("This todo is already in Done state");
        }

        Done = true;

        RegisterDomainEvent(new TodoIsDoneEvent(Id));
        return this;
    }
}
