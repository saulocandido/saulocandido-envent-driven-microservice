using Common.eventDriven.Interfaces;
using Common.EventDriven.Interfaces;


namespace Common.EventDriven.Abistraction.MessageBus
{
    public abstract class IntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler<TIntegrationEvent>, IIntegrationEventHandler where TIntegrationEvent : IIntegrationEvent
    {
        //
        // Summary:
        //     Event handler topic.
        public string Topic { get; set; }

        //
        // Summary:
        //     IntegrationEventHandler constructor.
        protected IntegrationEventHandler()
        {
            Topic = typeof(TIntegrationEvent).Name;
        }

        //
        // Summary:
        //     IntegrationEventHandler constructor.
        //
        // Parameters:
        //   topic:
        //     Event handler topic.
        protected IntegrationEventHandler(string topic)
        {
            Topic = topic;
        }

        public abstract Task HandleAsync(TIntegrationEvent @event);

        public virtual Task HandleAsync(IIntegrationEvent? @event)
        {
            return HandleAsync((TIntegrationEvent)@event);
        }
    }
}
