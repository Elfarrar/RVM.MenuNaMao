using System.Text;
using System.Text.Json;
using RVM.MenuNaMao.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RVM.MenuNaMao.Infrastructure.RabbitMQ;

public sealed class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqPublisher> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task PublishAsync<T>(string queue, T message, CancellationToken ct = default)
    {
        try
        {
            var channel = await GetChannelAsync(ct);
            await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queue,
                mandatory: false,
                body: new ReadOnlyMemory<byte>(body),
                cancellationToken: ct);

            _logger.LogDebug("Published message to queue {Queue}", queue);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish message to queue {Queue}. RabbitMQ may be offline", queue);
        }
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken ct)
    {
        if (_channel is not null) return _channel;

        await _semaphore.WaitAsync(ct);
        try
        {
            if (_channel is not null) return _channel;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
            return _channel;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
        _semaphore.Dispose();
    }
}
