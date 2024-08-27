using Common.CQRS.Abstration.command;

namespace CustomerService.Domain.CustomerAggregate.Commands;

public record CreateCustomer(Customer? Entity) : Command<Customer>(Entity);
