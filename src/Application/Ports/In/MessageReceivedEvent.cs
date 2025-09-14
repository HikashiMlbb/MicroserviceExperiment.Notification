namespace Application.Ports.In;

public record MessageReceivedEvent(string Topic, string Body, string Recipient);