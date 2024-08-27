using Common.DDD.Abstration.Entities;
using Common.DDD.Abstration.Events;


namespace Common.CQRS.Abstration.command
{
    //
    // Summary:
    //     Processes a command by generating a domain event.
    //
    // Type parameters:
    //   TCommand:
    //     Command type.
    //
    //   TDomainEvent:
    //     Domain event type.
    //
    //   TEntity:
    //     Entity type.
    public interface ICommandProcessor<in TCommand, TEntity, out TDomainEvent> where TCommand : class, ICommand<TEntity> where TEntity : Entity where TDomainEvent : IDomainEvent
    {
        //
        // Summary:
        //     Process specified command by creating a domain event.
        //
        // Parameters:
        //   command:
        //     The command to process.
        //
        // Returns:
        //     {TDomainEvent} event resulting from the command.
        TDomainEvent Process(TCommand command);
    }

    //
    // Summary:
    //     Processes a command by generating a domain event.
    //
    // Type parameters:
    //   TCommand:
    //     Command type.
    //
    //   TDomainEvent:
    //     Domain event type.
    public interface ICommandProcessor<in TCommand, out TDomainEvent> where TCommand : class, ICommand where TDomainEvent : IDomainEvent
    {
        //
        // Summary:
        //     Process specified command by creating a domain event.
        //
        // Parameters:
        //   command:
        //     The command to process.
        //
        // Returns:
        //     {TDomainEvent} event resulting from the command.
        TDomainEvent Process(TCommand command);
    }
}
