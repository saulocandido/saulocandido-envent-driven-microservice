using Common.CQRS.Abstration.command;

namespace CustomerService.Domain.CustomerAggregate.Commands;

public record RemoveCustomer(Guid EntityId) : Command(EntityId);