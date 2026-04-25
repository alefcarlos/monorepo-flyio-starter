using Ardalis.Result;
using Flyio.Demo.ApiService.Entities;
using Mediator;

namespace Flyio.Demo.ApiService.UseCases.GetAll;

public class GetTodosHandler : IQueryHandler<GetAllTodosQuery, Result<List<TodoEntity>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetTodosHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<List<TodoEntity>>> Handle(GetAllTodosQuery query, CancellationToken cancellationToken)
    {
        var list = _dbContext.Todos.ToList();

        return Result.Success(list);
    }
}