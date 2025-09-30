using Application.Common;
using Application.Ports.Out;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Adapters.Out;

public class SmtpService : ISmtpService
{
    private readonly MailSettings _mailSettings;
    private readonly SmtpClient _client;
    private bool _isConnected = false;
    private bool _isDisposed = false;
    
    public SmtpService(MailSettings mailSettings)
    {
        _mailSettings = mailSettings;
        _client = new SmtpClient();
    }
    
    public async Task SendMessage(Message message)
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(SmtpService));

        try
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_mailSettings.EmailName, _mailSettings.EmailAddress));
            mimeMessage.To.Add(new MailboxAddress(message.Recipient, message.Recipient));
            mimeMessage.Subject = message.Topic;
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message.Body
            };

            if (!_isConnected) await Connect();
            await _client.SendAsync(mimeMessage); 
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task Connect()
    {
        _isConnected = true;
        await _client.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, _mailSettings.UseSsl);
        await _client.AuthenticateAsync(_mailSettings.SmtpLogin, _mailSettings.SmtpPassword);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        
        await _client.DisconnectAsync(true);
        GC.SuppressFinalize(this);
    }
}