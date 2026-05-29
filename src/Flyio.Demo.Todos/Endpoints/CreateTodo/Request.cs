using System.ComponentModel.DataAnnotations;

namespace Flyio.Demo.Todos.Endpoints.CreateTodo;

public record CreateTodoRequest([Required]string Name);
