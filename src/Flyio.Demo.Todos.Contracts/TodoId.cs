namespace Flyio.Demo.Todos.Contracts;

public readonly record struct TodoId(Guid Value)
{
    public static TodoId Empty { get; } = new(Guid.Empty);
    public static TodoId NewTodoId() => new(Guid.CreateVersion7());
}