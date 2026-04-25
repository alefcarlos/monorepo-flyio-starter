using Ardalis.Result;
using Flyio.Demo.ApiService.Entities;
using Mediator;

namespace Flyio.Demo.ApiService.UseCases.Get;

public class GetTodoHandler : IQueryHandler<GetTodoQuery, Result<TodoEntity>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetTodoHandler(IApplicationDbContext dbContext)
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
