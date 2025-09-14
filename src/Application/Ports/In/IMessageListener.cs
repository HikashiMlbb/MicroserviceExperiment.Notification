namespace Application.Ports.In;

public interface IMessageListener
{
    public Task StartListeningAsync(CancellationToken token);
    public event MessageHandler? OnMessageReceived;
    
    public delegate Task MessageHandler(MessageReceivedEvent messageReceivedEvent);
}