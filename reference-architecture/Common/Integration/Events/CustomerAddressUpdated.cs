using Common.eventDriven.Abistraction.MessageBus;
using Common.Integration.Models;

namespace Common.Integration.Events
{
    public record CustomerAddressUpdated(Guid CustomerId, Address ShippingAddress) : IntegrationEvent;
}