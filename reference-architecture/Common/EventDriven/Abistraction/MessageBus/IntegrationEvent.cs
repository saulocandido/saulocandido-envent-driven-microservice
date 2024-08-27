using Common.eventDriven.Interfaces;
using Common.EventDriven.Interfaces;
using MediatR;

namespace Common.eventDriven.Abistraction.MessageBus;

public record IntegrationEvent : IIntegrationEvent, INotification
{
    public string Id { get; set; } = Guid.NewGuid().ToString();


    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    public string EventType { get; set; }

    protected IntegrationEvent()
    {
        EventType = GetType().FullName; 
    }

}

