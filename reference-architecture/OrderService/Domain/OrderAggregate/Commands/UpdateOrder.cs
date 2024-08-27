
using Common.CQRS.Abstration.command;

namespace OrderService.Domain.OrderAggregate.Commands;

public record UpdateOrder(Order Entity) : Command<Order>(Entity);