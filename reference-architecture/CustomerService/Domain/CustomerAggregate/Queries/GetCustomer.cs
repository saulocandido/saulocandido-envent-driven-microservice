using Common.CQRS.Abstration.Queries;

namespace CustomerService.Domain.CustomerAggregate.Queries;

public record GetCustomer(Guid Id) : Query<Customer?>;