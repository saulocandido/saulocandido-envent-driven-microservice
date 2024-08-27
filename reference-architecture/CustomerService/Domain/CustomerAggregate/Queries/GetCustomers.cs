using Common.CQRS.Abstration.Queries;

namespace CustomerService.Domain.CustomerAggregate.Queries;

public record GetCustomers : Query<IEnumerable<Customer>>;