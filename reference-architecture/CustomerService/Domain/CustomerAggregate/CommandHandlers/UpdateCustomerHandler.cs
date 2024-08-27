using AutoMapper;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using MediatR;
using Common.Integration.Events;
using Common.EventDriven.Interfaces;
using Common.CQRS.Abstration.command;
using Common.CQRS.Abstration;
using Common.CQRS.Abstration.Exceptions;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers
{

    /// <summary>
    /// Handles the <see cref="UpdateCustomer"/> command.
    /// </summary>
    public class UpdateCustomerHandler : ICommandHandler<Customer, UpdateCustomer>
    {
        private readonly ICustomerRepository _repository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCustomerHandler> _logger;
        private readonly IEventBus _eventBus;

        public UpdateCustomerHandler(
            ICustomerRepository repository,
            IMediator mediator,
            IMapper mapper,
            ILogger<UpdateCustomerHandler> logger,
            IEventBus eventBus)
        {
            _repository = repository;
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }


        /// <summary>
        /// Handles the <see cref="UpdateCustomer"/> command.
        /// </summary>
        /// <param name="command">the command.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>the result of the command.</returns>
        public async Task<CommandResult<Customer>> Handle(UpdateCustomer command, CancellationToken cancellationToken)
        {

            // Process command
            if (command.Entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
            var domainEvent = command.Entity.Process(command);

            // Apply events
            command.Entity.Apply(domainEvent);

            // Compare shipping addresses
            var existing = await _repository.GetAsync(command.EntityId);
            if (existing == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
            var addressChanged = command.Entity.ShippingAddress != existing.ShippingAddress;

            try
            {
                // Persist entity
                var entity = await _repository.UpdateAsync(command.Entity);
                if (entity == null) return new CommandResult<Customer>(CommandOutcome.NotFound);

                // Publish events
                if (addressChanged)
                {
                    var shippingAddress = _mapper.Map<Common.Integration.Models.Address>(entity.ShippingAddress);
                    _logger.LogInformation("----- Publishing event: {EventName}", $"v1.{nameof(CustomerAddressUpdated)}");
                    await _eventBus.PublishAsync(
                            new CustomerAddressUpdated(command.Entity.Id, shippingAddress), nameof(CustomerAddressUpdated),
                             "v1");
                }
                return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
            }
            catch (ConcurrencyException)
            {
                return new CommandResult<Customer>(CommandOutcome.Conflict);
            }
        }
    }
}
