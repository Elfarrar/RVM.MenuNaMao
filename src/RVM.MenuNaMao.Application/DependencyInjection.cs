using System.Reflection;
using RVM.MenuNaMao.Application.Behaviors;
using RVM.MenuNaMao.Application.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace RVM.MenuNaMao.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMenuNaMaoApplication(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator.Mediator>();

        var assembly = Assembly.GetExecutingAssembly();

        // Register all IRequestHandler<,> implementations
        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .Select(i => (Interface: i, Implementation: t)));

        foreach (var (iface, impl) in handlerTypes)
            services.AddScoped(iface, impl);

        // Register all INotificationHandler<> implementations
        var notificationHandlerTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                .Select(i => (Interface: i, Implementation: t)));

        foreach (var (iface, impl) in notificationHandlerTypes)
            services.AddScoped(iface, impl);

        // Register pipeline behaviors (order matters: logging wraps validation)
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
