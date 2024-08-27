using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Common.EventDriven.Interfaces;
using Polly;
using Common.eventDriven.Abistraction.MessageBus;
using Common.eventDriven.Interfaces;

namespace Common.EventDriven.Abstraction.MessageBus
{
    /// <summary>
    /// The EventBus class is responsible for publishing and subscribing to events using RabbitMQ.
    /// </summary>
    public class EventBus : IEventBus, IAsyncDisposable
    {
        /// <summary>
        /// Logger for the EventBus.
        /// </summary>
        private readonly ILogger<EventBus> _logger;

        /// <summary>
        /// The name of the exchange used for publishing and subscribing to events.
        /// </summary>
        private readonly string _exchangeName;

        /// <summary>
        /// The RabbitMQ connection.
        /// </summary>
        private readonly IConnection _connection;

        /// <summary>
        /// The RabbitMQ channel used for communication.
        /// </summary>
        private readonly IModel _channel;

        /// <summary>
        /// A thread-safe dictionary to store topics and their associated event handlers.
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentBag<IIntegrationEventHandler>> _topics = new();

        /// <summary>
        /// A cache to store event types for faster deserialization.
        /// </summary>
        private readonly ConcurrentDictionary<string, Type> _typeCache = new();

        /// <summary>
        /// The key used to store the event type in message headers.
        /// </summary>
        private const string EventTypeKey = "EventType";

        private readonly Dictionary<string, Type> _knownTypes = new Dictionary<string, Type>();

        private readonly EventBusOptions _options;

        /// <summary>
        /// Constructor for the EventBus.
        /// </summary>
        /// <param name="logger">Logger for the EventBus.</param>
        /// <param name="options">Configuration options for the EventBus.</param>
        public EventBus(ILogger<EventBus> logger, IOptions<EventBusOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _exchangeName = _options.PubSubName;

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, true);
        }

        /// <summary>
        /// Set of topics and their subscribers.
        /// </summary>
        public Dictionary<string, List<IIntegrationEventHandler>> Topics
        {
            get
            {
                return _topics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
            }
        }

        /// <summary>
        /// Resolves the Type based on the type name.
        /// </summary>
        /// <param name="typeName">The name of the type to resolve.</param>
        /// <returns>The resolved Type, or null if not found.</returns>
        private Type ResolveType(string typeName)
        {
            if (_knownTypes.TryGetValue(typeName, out var type))
            {
                return type;
            }

            type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(typeName))
                .FirstOrDefault(t => t != null);

            if (type != null)
            {
                _knownTypes[typeName] = type;
            }

            return type;
        }

        /// <summary>
        /// Subscribes an event handler to a specific topic.
        /// </summary>
        /// <param name="handler">The event handler to subscribe.</param>
        /// <param name="topic">The topic to subscribe to.</param>
        /// <param name="prefix">Optional prefix for the topic.</param>
        /// <param name="suffix">Optional suffix for the topic.</param>
        public void Subscribe(IIntegrationEventHandler handler, string? topic = null, string? prefix = null, string? suffix = null)
        {
            var fullTopic = BuildTopicName(topic, prefix, suffix);
            var handlers = _topics.GetOrAdd(fullTopic, _ => new ConcurrentBag<IIntegrationEventHandler>());
            handlers.Add(handler);

            var serviceName = AppDomain.CurrentDomain.FriendlyName;
            var queueName = $"{serviceName}.{fullTopic}";

            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queueName, _exchangeName, fullTopic);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());
                await HandleIncomingEvent(fullTopic, message);
            };
            _channel.BasicConsume(queueName, true, consumer);

            _logger.LogInformation("Subscribed to topic: {Topic} with queue {QueueName}", fullTopic, queueName);
        }

        /// <summary>
        /// Unsubscribes an event handler from a specific topic.
        /// </summary>
        /// <param name="handler">The event handler to unsubscribe.</param>
        /// <param name="topic">The topic to unsubscribe from.</param>
        /// <param name="prefix">Optional prefix for the topic.</param>
        /// <param name="suffix">Optional suffix for the topic.</param>
        public void UnSubscribe(IIntegrationEventHandler handler, string? topic = null, string? prefix = null, string? suffix = null)
        {
            var fullTopic = BuildTopicName(topic, prefix, suffix);
            if (_topics.TryGetValue(fullTopic, out var handlers))
            {
                handlers.TryTake(out _);
                if (!handlers.Any())
                {
                    _topics.TryRemove(fullTopic, out _);
                    _channel.QueueUnbind(_channel.QueueDeclare().QueueName, _exchangeName, fullTopic);
                }
            }
            _logger.LogInformation("Unsubscribed from topic: {Topic}", fullTopic);
        }

        /// <summary>
        /// Publishes an event to a specific topic.
        /// </summary>
        /// <typeparam name="TIntegrationEvent">The type of the event to publish.</typeparam>
        /// <param name="event">The event to publish.</param>
        /// <param name="topic">The topic to publish to.</param>
        /// <param name="prefix">Optional prefix for the topic.</param>
        /// <param name="suffix">Optional suffix for the topic.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event, string? topic = null, string? prefix = null, string? suffix = null, CancellationToken cancellationToken = default) where TIntegrationEvent : IntegrationEvent
        {
            var fullTopic = BuildTopicName(topic, prefix, suffix);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = new Dictionary<string, object> { { EventTypeKey, @event.GetType().FullName } };

            var retryPolicy = Policy
                .Handle<RabbitMQ.Client.Exceptions.BrokerUnreachableException>()
                .Or<RabbitMQ.Client.Exceptions.OperationInterruptedException>()
                .Or<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} due to {Exception}. Retrying in {TimeSpan}", retryCount, exception.GetType().Name, timeSpan);
                    });

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1),
                    onBreak: (ex, breakDelay) => _logger.LogWarning("Circuit breaker triggered due to {Exception}", ex.GetType().Name),
                    onReset: () => _logger.LogInformation("Circuit breaker reset"));

            await circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await Task.Run(() => _channel.BasicPublish(_exchangeName, fullTopic, properties, body), cancellationToken);
                    _logger.LogInformation("Published event to topic: {Topic}", fullTopic);
                });
            });
        }

        /// <summary>
        /// Handles incoming events for a specific topic.
        /// </summary>
        /// <param name="topicName">The name of the topic.</param>
        /// <param name="eventData">The serialized event data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task HandleIncomingEvent(string topicName, string eventData)
        {
            if (!_topics.TryGetValue(topicName, out var handlers))
            {
                _logger.LogWarning("No handlers found for topic: {Topic}", topicName);
                return;
            }

            var @event = DeserializeEvent(eventData, topicName);
            if (@event == null) return;

            var handlerTasks = handlers.Select(handler => handler.HandleAsync(@event)).ToList();
            await Task.WhenAll(handlerTasks);
        }

        /// <summary>
        /// Deserializes an event from its JSON representation.
        /// </summary>
        /// <param name="eventData">The serialized event data.</param>
        /// <param name="topicName">The name of the topic.</param>
        /// <returns>The deserialized event, or null if deserialization fails.</returns>
        private IIntegrationEvent? DeserializeEvent(string eventData, string topicName)
        {
            try
            {
                using var jsonDocument = JsonDocument.Parse(eventData);
                var root = jsonDocument.RootElement;
                if (!root.TryGetProperty(EventTypeKey, out var eventTypeElement))
                {
                    _logger.LogWarning("Received event without EventType for topic {Topic}. Raw data: {EventData}", topicName, eventData);
                    return null;
                }

                var eventTypeName = eventTypeElement.GetString();
                if (string.IsNullOrEmpty(eventTypeName))
                {
                    _logger.LogWarning("EventType is null or empty for topic {Topic}", topicName);
                    return null;
                }

                var eventType = ResolveType(eventTypeName);

                if (eventType == null || !typeof(IIntegrationEvent).IsAssignableFrom(eventType))
                {
                    _logger.LogWarning("Unknown or invalid event type {EventType} for topic {Topic}", eventTypeName, topicName);
                    return null;
                }

                var @event = JsonSerializer.Deserialize(eventData, eventType) as IIntegrationEvent;
                if (@event == null)
                {
                    _logger.LogError("Failed to deserialize event for topic {Topic}", topicName);
                    return null;
                }
                return @event;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing event for topic {Topic}. Raw data: {EventData}", topicName, eventData);
                return null;
            }
        }

        /// <summary>
        /// Builds a full topic name from its components.
        /// </summary>
        /// <param name="topic">The main topic name.</param>
        /// <param name="prefix">Optional prefix for the topic.</param>
        /// <param name="suffix">Optional suffix for the topic.</param>
        /// <returns>The full topic name.</returns>
        private static string BuildTopicName(string? topic, string? prefix, string? suffix)
        {
            if (string.IsNullOrEmpty(topic) && string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(suffix))
            {
                throw new ArgumentException("At least one of 'topic', 'prefix', or 'suffix' must be provided.");
            }

            var parts = new List<string>();
            if (!string.IsNullOrEmpty(prefix)) parts.Add(prefix);
            if (!string.IsNullOrEmpty(topic)) parts.Add(topic);
            if (!string.IsNullOrEmpty(suffix)) parts.Add(suffix);
            return string.Join(".", parts);
        }

        /// <summary>
        /// Gets the number of subscribers for a specific topic.
        /// </summary>
        /// <param name="topic">The topic to check.</param>
        /// <returns>The number of subscribers for the topic.</returns>
        public int GetSubscriberCount(string topic) => _topics.TryGetValue(topic, out var handlers) ? handlers.Count : 0;

        /// <summary>
        /// Disposes of the EventBus, releasing all resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Core disposal method for asynchronous disposal pattern.
        /// </summary>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_channel is not null)
            {
                await Task.Run(() => _channel.Close());
                _channel.Dispose();
            }

            if (_connection is not null)
            {
                await Task.Run(() => _connection.Close());
                _connection.Dispose();
            }
        }
    }
}
