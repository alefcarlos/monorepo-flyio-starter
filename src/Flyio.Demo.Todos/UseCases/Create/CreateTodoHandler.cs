using Ardalis.Result;
using Flyio.Demo.Todos.Contracts;
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

    public async ValueTask<Result<TodoEntity>> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        return Result.Invalid(new ValidationError("aconteceu um erro fodase"));

        // var entity = TodoEntity.CreateNew(command.Name);

        // _dbContext.Todos.Add(entity);
        // await _dbContext.SaveChangesAsync(cancellationToken);

        // return Result.Created(Result.Success(entity));
    }
}