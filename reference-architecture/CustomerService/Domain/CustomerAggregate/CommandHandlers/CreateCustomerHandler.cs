using Common.CQRS.Abstration;
using Common.CQRS.Abstration.command;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using MediatR;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers;

public class CreateCustomerHandler : ICommandHandler<Customer, CreateCustomer>
{
    private readonly ICustomerRepository _repository;
    private readonly IMediator _mediator;

    public CreateCustomerHandler(ICustomerRepository repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    public async Task<CommandResult<Customer>> Handle(CreateCustomer command, CancellationToken cancellationToken)
    {
        if (command.Entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        var domainEvent = command.Entity.Process(command);
        command.Entity.Apply(domainEvent);
        var entity = await _repository.AddAsync(command.Entity);
        if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);

        // Publish the domain event
        await _mediator.Publish(domainEvent, cancellationToken);

        return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
    }
}