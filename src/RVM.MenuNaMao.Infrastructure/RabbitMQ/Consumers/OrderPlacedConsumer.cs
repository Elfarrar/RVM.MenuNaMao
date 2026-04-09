using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RVM.MenuNaMao.Infrastructure.RabbitMQ.Consumers;

public sealed class OrderPlacedConsumer(IOptions<RabbitMqSettings> settings, ILogger<OrderPlacedConsumer> logger)
    : BackgroundService
{
    private const string QueueName = "orders.placed";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = settings.Value.HostName,
                Port = settings.Value.Port,
                UserName = settings.Value.UserName,
                Password = settings.Value.Password
            };

            var connection = await factory.CreateConnectionAsync(stoppingToken);
            var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
            await channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body.Span);
                logger.LogInformation("[{Queue}] Received: {Message}", QueueName, body);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, stoppingToken);
            };

            await channel.BasicConsumeAsync(QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "RabbitMQ consumer {Queue} failed to start. RabbitMQ may be offline", QueueName);
        }
    }
}
