using Ardalis.Result;
using Flyio.Demo.Todos.Domain;
using Flyio.Demo.Todos.Infra;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Flyio.Demo.Todos.UseCases.GetAll;

internal class GetTodosHandler : IQueryHandler<GetAllTodosQuery, Result<List<TodoEntity>>>
{
    private readonly ITodosDbContext _dbContext;

    public GetTodosHandler(ITodosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<List<TodoEntity>>> Handle(GetAllTodosQuery query, CancellationToken cancellationToken)
    {
        var list = await _dbContext.Todos.ToListAsync(cancellationToken: cancellationToken);

        return Result.Success(list);
    }
}