using Application.Common;
using Application.Ports.Out;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Adapters.Out;

public class MailKitEmailSender(ISmtpService smtp) : IEmailSender
{
    public async Task SendEmailAsync(string topic, string body, string recipient)
    {
        var message = new Message(topic, body, recipient);
        await smtp.SendMessage(message);
    }
}