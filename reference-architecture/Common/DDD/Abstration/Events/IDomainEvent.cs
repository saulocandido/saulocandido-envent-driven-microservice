using Common.Interfaces;
using MediatR;

namespace Common.DDD.Abstration.Events
{
    public interface IDomainEvent : IEvent
    {
        //
        // Summary:
        //     The id of the entity that this event is about.
        Guid EntityId { get; }

        //
        // Summary:
        //     The etag of the entity that this event is about.
        string? EntityETag { get; }
    }

    //
    // Summary:
    //     A statement of fact about what change has been made to the domain state.
    //
    // Type parameters:
    //   TEntity:
    //     Entity type.
    public interface IDomainEvent<out TEntity> : IDomainEvent, IEvent where TEntity : IEntity
    {
        //
        // Summary:
        //     The entity the domain event is in reference to.
        TEntity? Entity { get; }
    }
}
