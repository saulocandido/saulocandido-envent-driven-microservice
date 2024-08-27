
using Common.CQRS.Abstration.command;

namespace OrderService.Domain.OrderAggregate.Commands;

public record ShipOrder(Guid EntityId) : Command<Order>(null, EntityId);