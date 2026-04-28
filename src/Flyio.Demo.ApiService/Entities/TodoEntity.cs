namespace Flyio.Demo.ApiService.Entities;

public readonly record struct TodoId(Guid Value)
{
    public static TodoId Empty { get; } = new(Guid.Empty);
    public static TodoId NewTodoId() => new(Guid.NewGuid());
}

public class TodoEntity
{
    public TodoId Id { get; internal set; }
    public string Name { get; internal set; } = string.Empty;

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
}