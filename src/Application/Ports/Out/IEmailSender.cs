namespace Application.Ports.Out;

public interface IEmailSender
{
    public Task SendEmailAsync(string topic, string body, string recipient);
}