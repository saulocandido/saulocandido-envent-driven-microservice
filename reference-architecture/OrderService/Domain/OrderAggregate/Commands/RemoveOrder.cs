
using Common.CQRS.Abstration.command;

namespace OrderService.Domain.OrderAggregate.Commands;

public record RemoveOrder(Guid EntityId) : Command(EntityId);