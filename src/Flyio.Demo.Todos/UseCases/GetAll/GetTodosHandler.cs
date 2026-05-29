using Ardalis.Result;
using Flyio.Demo.Todos.Domain;
using Flyio.Demo.Todos.Infra;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.GetAll;

public class GetTodosHandler : IQueryHandler<GetAllTodosQuery, Result<List<TodoEntity>>>
{
    private readonly TodosDbContext _dbContext;

    public GetTodosHandler(TodosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<List<TodoEntity>>> Handle(GetAllTodosQuery query, CancellationToken cancellationToken)
    {
        var list = _dbContext.Todos.ToList();

        return Result.Success(list);
    }
}