namespace Infrastructure.Settings;

public sealed class RabbitMQSettings
{
    public string ConnectionString { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}