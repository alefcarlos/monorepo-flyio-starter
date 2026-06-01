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

    public static TodoEntity CreateNew(string name)
    {
        var entity = new TodoEntity
        {
            Id = TodoId.NewTodoId(),
            Name = name
        };

        return entity;
    }

    public Result SetDone()
    {
        if (Done)
        {
            //add validation
            return Result.Error("This todo is already in Done state");
        }

        Done = true;

        RegisterDomainEvent(new TodoIsDoneEvent(Id));
        return Result.Success();
    }
}
