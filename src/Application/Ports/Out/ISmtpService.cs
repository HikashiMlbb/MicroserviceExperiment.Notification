using Application.Common;

namespace Application.Ports.Out;

public interface ISmtpService : IAsyncDisposable
{
    public Task SendMessage(Message message);
}