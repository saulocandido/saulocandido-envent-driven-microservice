

using Common.CQRS.Abstration.command;

namespace OrderService.Domain.OrderAggregate.Commands;

public record CancelOrder(Guid EntityId) : Command<Order>(null, EntityId);