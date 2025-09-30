using Application.Common;
using Application.Ports.Out;
using Infrastructure.Adapters.Out;
using Moq;

namespace Infrastructure.Tests;

[TestFixture]
public class MailKitEmailSenderTests
{
    [Test]
    public async Task Init_Test()
    {
        var smtpServiceMock = new Mock<ISmtpService>();
        smtpServiceMock.Setup(x => x.SendMessage(It.IsAny<Message>())).Returns(Task.CompletedTask);
        var sender = new MailKitEmailSender(smtpServiceMock.Object);

        const string topic = "Example topic";
        const string body = "Some interesting body";
        const string recipient = "some_interesting_recipient@mail.com";
        
        await sender.SendEmailAsync(topic, body, recipient);

        smtpServiceMock.Verify(x => x.SendMessage(new Message(topic, body, recipient)), Times.Once);
        Assert.Pass();
    }
}