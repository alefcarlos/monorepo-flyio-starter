using Flyio.Demo.SharedKernel.Entities;
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

    public void SetDone()
    {
        Done = true;

        RegisterDomainEvent(new TodoIsDoneEvent(Id));
    }
}
