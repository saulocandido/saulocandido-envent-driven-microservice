using Common.eventDriven.Interfaces;

namespace Common.EventDriven.Interfaces;


public interface IIntegrationEventHandler
{
    //
    // Summary:
    //     Handler topic.
    string Topic { get; set; }

    //
    // Summary:
    //     Handle an event asynchronously.
    //
    // Parameters:
    //   event:
    //     Integration event.
    //
    // Returns:
    //     Task that will complete when the operation has completed.
    Task HandleAsync(IIntegrationEvent @event);
}

//
// Summary:
//     Handler for integration events.
//
// Type parameters:
//   TIntegrationEvent:
//     Integration event type.
public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler where TIntegrationEvent : IIntegrationEvent
{
    Task HandleAsync(TIntegrationEvent @event);
}