using RabbitMQ.Client;
using StorageMicroservice.Infrastructure.EventBus;
using System.Text;
using System.Text.Json;
using Polly;
using Polly.Retry;
using System.Collections.Concurrent;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Configuration;


public class RabbitMQEventBus : IEventBus
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ConcurrentDictionary<string, Type> _handlers;
    private readonly IConfiguration configuration;
    public RabbitMQEventBus()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            ConsumerDispatchConcurrency = 2,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };
        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
        _channel.ExchangeDeclareAsync(exchange: "event_bus", type: ExchangeType.Fanout).Wait();
        _retryPolicy = Policy.Handle<Exception>().RetryAsync(3);
        _handlers = new ConcurrentDictionary<string, Type>();
    }



    public async Task PublishAsync<T>(T @event)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
        await _channel.BasicPublishAsync("event_bus", "",true, body);
    }
    public async Task SubscribeAsync<T, TH>() where T : class where TH : IEventHandler<T>
    {
           var eventName = typeof(T).Name;
           _handlers.TryAdd(eventName, typeof(TH));
            var queueName =(await _channel.QueueDeclareAsync()).QueueName;
           await _channel.QueueBindAsync(queue: queueName, exchange: "event_bus", routingKey: "");
           var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            if (_handlers.TryGetValue(eventName, out var handlerType))
            {
                var handler = Activator.CreateInstance(handlerType);
                var eventData = System.Text.Json.JsonSerializer.Deserialize(message, handlerType);
                await ((IEventHandler<T>)handler).Handle((T)eventData);
            }
            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };
            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
     }

}
