
using Common.CQRS.Abstration.Event;

namespace OrderService.Domain.OrderAggregate.Events;

public record OrderCancelled(Guid EntityId) : DomainEvent<Order>(null, EntityId);