
using Common.CQRS.Abstration.Queries;

namespace OrderService.Domain.OrderAggregate.Queries;

public record GetOrders : Query<IEnumerable<Order>>;