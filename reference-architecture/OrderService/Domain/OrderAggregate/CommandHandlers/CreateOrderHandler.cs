using Common.CQRS.Abstration;
using Common.CQRS.Abstration.command;
using Common.EventDriven.Interfaces;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.CommandHandlers;

public class CreateOrderHandler : ICommandHandler<Order, CreateOrder>
{
    private readonly IOrderRepository _repository;
    IEventBus _eventBus;
    public CreateOrderHandler(
        IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult<Order>> Handle(CreateOrder command, CancellationToken cancellationToken)
    {
        // Process command
        if (command.Entity == null) return new CommandResult<Order>(CommandOutcome.InvalidCommand);
        var domainEvent = command.Entity.Process(command);
        // Apply events
        command.Entity.Apply(domainEvent);
            
        // Persist entity
        var entity = await _repository.AddAsync(command.Entity);
        if (entity == null) return new CommandResult<Order>(CommandOutcome.InvalidCommand);
        return new CommandResult<Order>(CommandOutcome.Accepted, entity);
    }
}