using Common.CQRS.Abstration.Event;

namespace CustomerService.Domain.CustomerAggregate.Events;

public record CustomerRemoved(Guid EntityId) : DomainEvent(EntityId);