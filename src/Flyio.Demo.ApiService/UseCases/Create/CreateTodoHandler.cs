using Ardalis.Result;
using Flyio.Demo.ApiService.Entities;
using Mediator;

namespace Flyio.Demo.ApiService.UseCases.Create;

public class CreateTodoHandler : ICommandHandler<CreateTodoCommand, Result>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateTodoHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        var entity = TodoEntity.CreateNew(command.Name);

        _dbContext.Todos.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Created(Result.Success());
    }
}