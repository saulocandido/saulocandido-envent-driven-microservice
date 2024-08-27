

using Common.CQRS.Abstration.command;

namespace OrderService.Domain.OrderAggregate.Commands;

public record CreateOrder(Order Entity) : Command<Order>(Entity);