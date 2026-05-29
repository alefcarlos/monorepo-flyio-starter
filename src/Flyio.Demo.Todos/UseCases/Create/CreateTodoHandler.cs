using Ardalis.Result;
using Flyio.Demo.Todos.Entities;
using Flyio.Demo.Todos.Infra;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.Create;

public class CreateTodoHandler : ICommandHandler<CreateTodoCommand, Result<TodoId>>
{
    private readonly TodosDbContext _dbContext;

    public CreateTodoHandler(TodosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<TodoId>> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        var entity = TodoEntity.CreateNew(command.Name);

        _dbContext.Todos.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Created(Result.Success(entity.Id));
    }
}