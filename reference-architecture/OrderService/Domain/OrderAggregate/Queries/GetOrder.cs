
using Common.CQRS.Abstration.Queries;

namespace OrderService.Domain.OrderAggregate.Queries;

public record GetOrder(Guid Id) : Query<Order?>;