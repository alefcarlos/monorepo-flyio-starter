using Ardalis.Result;
using Flyio.Demo.Todos.Infra;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.SetDone;

internal class SetTodoDoneHandler : ICommandHandler<SetTodoDoneCommand, Result>
{
    private readonly ITodosDbContext _dbContext;

    public SetTodoDoneHandler(ITodosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result> Handle(SetTodoDoneCommand command, CancellationToken cancellationToken)
    {
        var entity = _dbContext.Todos.FirstOrDefault(x=>x.Id == command.Id);

        if (entity is null)
        {
            return Result.NotFound();
        }

        entity.SetDone();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}