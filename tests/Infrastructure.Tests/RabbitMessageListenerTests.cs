using System.Text;
using System.Text.Json;
using Application.Common;
using Infrastructure.Adapters.In;
using Infrastructure.Settings;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;

namespace Infrastructure.Tests;

[TestFixture]
public class RabbitMessageListenerTests
{
    private RabbitMqContainer _container;

    [OneTimeSetUp]
    public async Task SetUpOnce()
    {
        _container = new RabbitMqBuilder().WithImage("rabbitmq:4.1").Build();

        await _container.StartAsync();
    }

    [OneTimeTearDown]
    public async Task TearDownOnce()
    {
        await _container.DisposeAsync();
    }

    [Test]
    public async Task Init_Test()
    {
        var settings = new RabbitMQSettings
        {
            ConnectionString = _container.GetConnectionString(),
            QueueName = "test-queue-name"
        };
        var listener = new RabbitMessageListener(settings);

        await listener.DisposeAsync();
        Assert.Pass();
    }

    [Test]
    public async Task Init_Test_With_Listening()
    {
        await SendMessage();

        var messageReceivedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var settings = new RabbitMQSettings
        {
            ConnectionString = _container.GetConnectionString(),
            QueueName = "test-queue-name"
        };
        var listener = new RabbitMessageListener(settings);
        listener.OnMessageReceived += async (message) =>
        {
            await TestContext.Out.WriteLineAsync($"{message.Topic} -- {message.Body} -- {message.Recipient}");
            messageReceivedTcs.SetResult(true);
        };
            
        await listener.StartListeningAsync(CancellationToken.None);

        var result = await messageReceivedTcs.Task.WaitAsync(TimeSpan.FromSeconds(10));
        
        Assert.That(result, Is.True, "Failed :(");
        
        await listener.DisposeAsync();
    }

    private async Task SendMessage()
    {
        // TODO: make method that generates a message to a queue before any test starts
        var message = new Message("some-interesting-topic", "Some interesting body!!!", "some-interesting@recipient.com");
        var messsageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var factory = new ConnectionFactory { Uri = new Uri(_container.GetConnectionString()) };
        await using var con = await factory.CreateConnectionAsync();
        await using var chan = await con.CreateChannelAsync();

        await chan.QueueDeclareAsync("test-queue-name", true, false, false);
        await chan.BasicPublishAsync(String.Empty, "test-queue-name", body: messsageBytes);

        await chan.CloseAsync();
        await con.CloseAsync();
    }

}