using Common.CQRS.Abstration.command;
using Common.DDD.Abstration.Entities;
using Common.DDD.Abstration.Events;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;

namespace CustomerService.Domain.CustomerAggregate;

public class Customer :
    Entity,
    ICommandProcessor<CreateCustomer, Customer, CustomerCreated>,
    IEventApplier<CustomerCreated>,
    ICommandProcessor<UpdateCustomer, Customer, CustomerUpdated>,
    IEventApplier<CustomerUpdated>,
    ICommandProcessor<RemoveCustomer, CustomerRemoved>,
    IEventApplier<CustomerRemoved>
{

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Address ShippingAddress { get; set; } = null!;

    public CustomerCreated Process(CreateCustomer command)
    {
        // To process command, return one or more domain events
        return new CustomerCreated(this);
    }

    public void Apply(CustomerCreated domainEvent)
    {
        // Set Id if not already set
        if (Id == Guid.Empty)
        {
            Id = Guid.NewGuid();
        }
    }

    public CustomerUpdated Process(UpdateCustomer command)
    {
        // To process command, return a domain event
        return new CustomerUpdated(this);
    }

    public void Apply(CustomerUpdated domainEvent)
    {
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }

    public CustomerRemoved Process(RemoveCustomer command)
    {
        // To process command, return a domain event
        return new CustomerRemoved(Id);
    }

    public void Apply(CustomerRemoved domainEvent)
    {
        // Could implement soft delete logic here if needed
    }
}