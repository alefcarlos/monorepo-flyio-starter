using Ardalis.Result;
using Flyio.Demo.Todos.Domain;
using Flyio.Demo.Todos.Infra;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.Create;

internal class CreateTodoHandler : ICommandHandler<CreateTodoCommand, Result<TodoEntity>>
{
    private readonly ITodosDbContext _dbContext;

    public CreateTodoHandler(ITodosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD103:Call async methods when in an async method", Justification = "<Pending>")]
    public async ValueTask<Result<TodoEntity>> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        return await TodoEntity.CreateNew(command.Name)
            .BindAsync(async entity =>
            {
                _dbContext.Todos.Add(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return Result.Success(entity);
            })
            .BindAsync(entity => Result.Created(entity));
    }
}