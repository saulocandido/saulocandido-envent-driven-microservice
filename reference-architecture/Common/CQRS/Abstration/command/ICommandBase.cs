using Common.Interfaces;


namespace Common.CQRS.Abstration.command
{
    //
    // Summary:
    //     An object that is sent to the domain for a state change which is handled by a
    //     command handler.
    public interface ICommandBase
    {
        //
        // Summary:
        //     Represents the Id of the entity the command is in reference to.
        Guid EntityId { get; }

        //
        // Summary:
        //     Represents the ETag of the entity the command is in reference to.
        string? EntityETag { get; }
    }

    public interface ICommandBase<out TEntity> : ICommandBase where TEntity : IEntity
    {
        //
        // Summary:
        //     The entity the command is in reference to.
        TEntity? Entity { get; }
    }
}
