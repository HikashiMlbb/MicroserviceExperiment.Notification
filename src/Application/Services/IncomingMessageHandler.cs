using Application.Common;
using Application.Ports.Out;

namespace Application.Services;

public class IncomingMessageHandler(IEmailSender emailSender)
{
    public async Task Handle(Message message)
    {
        var topic = message.Topic;
        var body = message.Body;
        var recipient = message.Recipient;
        
        await emailSender.SendEmailAsync(topic, body, recipient);
    }
}