using Application.Common;
using Application.Ports.Out;
using Application.Services;
using Moq;

namespace Application.Tests;

[TestFixture]
public class IncomingMessageHandlerTests
{
    private Mock<IEmailSender> _emailSenderMock;
    private IncomingMessageHandler _handler;
    
    [SetUp]
    public void SetUp()
    {
        _emailSenderMock = new Mock<IEmailSender>();
        _handler = new IncomingMessageHandler(_emailSenderMock.Object);
    }

    [Test]
    public async Task SendMessage()
    {
        // Arrange
        const string topic = "Some interesting topic";
        const string body = "Some interesting message body...";
        const string recipient = "unknown.address@dark.com";
        var messageEvent = new Message(topic, body, recipient);

        _emailSenderMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        
        // Act
        await _handler.Handle(messageEvent);
        
        // Assert
        _emailSenderMock.Verify(x => x.SendEmailAsync(topic, body, recipient), Times.Once);
    }
}