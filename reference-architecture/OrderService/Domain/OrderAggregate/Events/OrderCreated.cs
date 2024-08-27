
using Common.CQRS.Abstration.Event;

namespace OrderService.Domain.OrderAggregate.Events;

public record OrderCreated(Order? Entity) : DomainEvent<Order>(Entity);