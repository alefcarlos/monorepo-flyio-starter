using Ardalis.Result;
using Flyio.Demo.Todos.Domain;
using Flyio.Demo.Todos.Infra;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Flyio.Demo.Todos.UseCases.Get;

internal class GetTodoHandler : IQueryHandler<GetTodoQuery, Result<TodoEntity>>
{
    private readonly ITodosDbContext _dbContext;

    public GetTodoHandler(ITodosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<TodoEntity>> Handle(GetTodoQuery query, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Todos.FirstOrDefaultAsync(x =>x.Id == query.Id, cancellationToken: cancellationToken);

        if (entity is null)
        {
            return Result.NotFound();
        }

        return Result.Success(entity);
    }
}
