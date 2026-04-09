namespace RVM.MenuNaMao.Application.Services;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(string queue, T message, CancellationToken ct = default);
}
