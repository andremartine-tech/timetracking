using Infrastructure.Messaging;

public class RabbitMqInitializer
{
    private readonly RabbitMqService _rabbitMqService;

    public RabbitMqInitializer(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    public async Task InitializeAsync(string queueName)
    {
        var channel = await _rabbitMqService.GetChannelAsync();
        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
    }
}