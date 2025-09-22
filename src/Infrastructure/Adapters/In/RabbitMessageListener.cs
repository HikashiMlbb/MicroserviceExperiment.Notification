using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common;
using Application.Ports.In;
using Infrastructure.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Adapters.In;

public class RabbitMessageListener : IMessageListener, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _queue;

    private bool _isDisposed = false;

    public RabbitMessageListener(RabbitMQSettings settings)
    {
        _queue = settings.QueueName;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(settings.ConnectionString)
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(_queue, true, false, false);
    }

    public async Task StartListeningAsync(CancellationToken token)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, @event) =>
        {
            var body = Encoding.UTF8.GetString(@event.Body.ToArray());
            var result = JsonSerializer.Deserialize<Message>(body)!;

            var message = new Message(result.Topic, result.Body, result.Recipient);

            OnMessageReceived?.Invoke(message);

            await _channel.BasicAckAsync(@event.DeliveryTag, false, token);
        };

        await _channel.BasicConsumeAsync(_queue, false, consumer, cancellationToken: token);
    }

    public event IMessageListener.MessageHandler? OnMessageReceived;

    private async Task ReleaseUnmanagedResources()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        await ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}