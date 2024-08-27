using Common.DDD.Abstration.Entities;
using MediatR;


namespace Common.CQRS.Abstration.command
{
    //
    // Summary:
    //     Command handler.
    //
    // Type parameters:s
    //     Command type.
    //
    //   TEntity:
    //     Entity type.
    public interface ICommandHandler<TEntity, in TCommand> : IRequestHandler<TCommand, CommandResult<TEntity>> where TEntity : Entity where TCommand : class, ICommand<TEntity>
    {
    }
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, CommandResult> where TCommand : class, ICommand
    {
    }

    //public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
    //{
    //}
}
