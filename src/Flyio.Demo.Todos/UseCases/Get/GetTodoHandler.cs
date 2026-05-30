using Ardalis.Result;
using Flyio.Demo.Todos.Domain;
using Flyio.Demo.Todos.Infra;
using Mediator;

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
        var entity = _dbContext.Todos.FirstOrDefault(x=>x.Id == query.Id);

        if (entity is null)
        {
            return Result.NotFound();
        }

        return Result.Success(entity);
    }
}
