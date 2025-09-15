using Application.Ports.In;
using Application.Ports.Out;

namespace Application.Services;

public class IncomingMessageHandler(IEmailSender emailSender)
{
    public async Task Handle(MessageReceivedEvent @event)
    {
        var topic = @event.Topic;
        var body = @event.Body;
        var recipient = @event.Recipient;
        
        await emailSender.SendEmailAsync(topic, body, recipient);
    }
}