using Ardalis.Result;
using Flyio.Demo.Todos.Domain;
using Flyio.Demo.Todos.Infra;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.SetDone;

internal class SetTodoDoneHandler : ICommandHandler<SetTodoDoneCommand, Result<TodoEntity>>
{
    private readonly ITodosDbContext _dbContext;

    public SetTodoDoneHandler(ITodosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<TodoEntity>> Handle(SetTodoDoneCommand command, CancellationToken cancellationToken)
    {
        var entity = _dbContext.Todos.FirstOrDefault(x => x.Id == command.Id);

        if (entity is null)
        {
            return Result.NotFound("Todo não encontrado");
        }

        return await entity.SetDone()
            .BindAsync(async e =>
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                return Result.Success(e);
            });
    }
}