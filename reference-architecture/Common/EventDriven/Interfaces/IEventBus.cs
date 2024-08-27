using Common.eventDriven.Abistraction.MessageBus;


namespace Common.EventDriven.Interfaces
{
    //
    // Summary:
    //     Provides a way for systems to communicate without knowing about each other.
    public interface IEventBus
    {
        //
        // Summary:
        //     List of topics with associated event handlers.
        Dictionary<string, List<IIntegrationEventHandler>> Topics { get; }

        //
        // Summary:
        //     Register a subscription with an event handler.
        //
        // Parameters:
        //   handler:
        //     Subscription event handler.
        //
        //   topic:
        //     Subscription topic.
        //
        //   prefix:
        //     Dot delimited prefix, which can include version.
        //
        //   suffix:
        //     Dot delimited suffix, which can include version.
        void Subscribe(IIntegrationEventHandler handler, string? topic = null, string? prefix = null, string? suffix = null);

        //
        // Summary:
        //     Unregister a subscription with an event handler.
        //
        // Parameters:
        //   handler:
        //     Subscription event handler.
        //
        //   topic:
        //     Subscription topic.
        //
        //   prefix:
        //     Dot delimited prefix, which can include version.
        //
        //   suffix:
        //     Dot delimited suffix, which can include version.
        void UnSubscribe(IIntegrationEventHandler handler, string? topic = null, string? prefix = null, string? suffix = null);

        //
        // Summary:
        //     Publish an event asynchronously.
        //
        // Parameters:
        //   event:
        //     Integration event.
        //
        //   topic:
        //     Publication topic.
        //
        //   prefix:
        //     Dot delimited prefix, which can include version.
        //
        //   suffix:
        //     Dot delimited suffix, which can include version.
        //
        // Type parameters:
        //   TIntegrationEvent:
        //     Integration event type.
        //
        // Returns:
        //     Task that will complete when the operation has completed.
        Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event, string? topic = null, string? prefix = null, string? suffix = null, CancellationToken cancellationToken = default)
            where TIntegrationEvent : IntegrationEvent;


        int GetSubscriberCount(string topic);
    }
}
