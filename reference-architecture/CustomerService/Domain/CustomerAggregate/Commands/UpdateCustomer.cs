using Common.CQRS.Abstration;
using Common.CQRS.Abstration.command;

namespace CustomerService.Domain.CustomerAggregate.Commands;

public record UpdateCustomer(Customer Entity) : Command<Customer>(Entity);