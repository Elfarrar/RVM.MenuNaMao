using Microsoft.Extensions.DependencyInjection;

namespace RVM.MenuNaMao.Application.Mediator;

public sealed class Mediator(IServiceProvider provider) : IMediator
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = provider.GetRequiredService(handlerType);

        var handleMethod = handlerType.GetMethod("Handle")!;

        // Build pipeline (Russian doll)
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = provider.GetServices(behaviorType).Reverse().ToList();

        RequestHandlerDelegate<TResponse> pipeline = ()
            => (Task<TResponse>)handleMethod.Invoke(handler, [request, ct])!;

        foreach (var behavior in behaviors)
        {
            var current = pipeline;
            var behaviorHandleMethod = behaviorType.GetMethod("Handle")!;
            pipeline = () => (Task<TResponse>)behaviorHandleMethod.Invoke(behavior, [request, current, ct])!;
        }

        return pipeline();
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken ct = default)
        where TNotification : INotification
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = provider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("Handle")!;
            await (Task)method.Invoke(handler, [notification, ct])!;
        }
    }
}
