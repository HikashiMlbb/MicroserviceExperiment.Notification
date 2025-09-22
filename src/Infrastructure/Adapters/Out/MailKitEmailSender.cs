using Application.Ports.Out;

namespace Infrastructure.Adapters.Out;

public class MailKitEmailSender : IEmailSender
{
    public Task SendEmailAsync(string topic, string body, string recipient)
    {
        throw new NotImplementedException();
    }
}