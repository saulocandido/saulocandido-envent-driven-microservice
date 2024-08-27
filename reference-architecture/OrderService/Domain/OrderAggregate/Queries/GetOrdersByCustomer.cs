
using Common.CQRS.Abstration.Queries;

namespace OrderService.Domain.OrderAggregate.Queries;

public record GetOrdersByCustomer(Guid CustomerId) : Query<IEnumerable<Order>>;