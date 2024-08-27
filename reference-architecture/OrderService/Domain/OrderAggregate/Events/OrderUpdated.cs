using Common.CQRS.Abstration.Event;

namespace OrderService.Domain.OrderAggregate.Events;

public record OrderUpdated(Order? Entity) : DomainEvent<Order>(Entity);