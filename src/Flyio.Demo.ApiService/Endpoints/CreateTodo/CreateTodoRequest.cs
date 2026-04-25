using System.ComponentModel.DataAnnotations;

namespace Flyio.Demo.ApiService.Endpoints.CreateTodo;

public record CreateTodoRequest([Required]string Name);
