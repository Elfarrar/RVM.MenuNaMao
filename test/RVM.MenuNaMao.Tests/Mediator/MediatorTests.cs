using RVM.MenuNaMao.Application.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace RVM.MenuNaMao.Tests.Mediator;

public class MediatorTests
{
    // Test request/handler
    public record PingRequest(string Message) : IRequest<string>;

    public class PingHandler : IRequestHandler<PingRequest, string>
    {
        public Task<string> Handle(PingRequest request, CancellationToken ct = default)
            => Task.FromResult($"Pong: {request.Message}");
    }

    // Test notification
    public record TestNotification(string Data) : INotification;

    public class TestNotificationHandler1 : INotificationHandler<TestNotification>
    {
        public static int CallCount;
        public Task Handle(TestNotification notification, CancellationToken ct = default)
        {
            Interlocked.Increment(ref CallCount);
            return Task.CompletedTask;
        }
    }

    public class TestNotificationHandler2 : INotificationHandler<TestNotification>
    {
        public static int CallCount;
        public Task Handle(TestNotification notification, CancellationToken ct = default)
        {
            Interlocked.Increment(ref CallCount);
            return Task.CompletedTask;
        }
    }

    // Test pipeline behavior
    public class TestBehavior : IPipelineBehavior<PingRequest, string>
    {
        public static int CallCount;
        public async Task<string> Handle(PingRequest request, RequestHandlerDelegate<string> next, CancellationToken ct = default)
        {
            Interlocked.Increment(ref CallCount);
            return await next();
        }
    }

    private static IMediator BuildMediator()
    {
        var services = new ServiceCollection();
        services.AddScoped<IMediator, Application.Mediator.Mediator>();
        services.AddScoped<IRequestHandler<PingRequest, string>, PingHandler>();
        services.AddScoped<INotificationHandler<TestNotification>, TestNotificationHandler1>();
        services.AddScoped<INotificationHandler<TestNotification>, TestNotificationHandler2>();
        services.AddScoped<IPipelineBehavior<PingRequest, string>, TestBehavior>();

        return services.BuildServiceProvider().GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task Send_ResolvesHandler_ReturnsResponse()
    {
        var mediator = BuildMediator();
        var result = await mediator.Send(new PingRequest("Hello"));
        Assert.Equal("Pong: Hello", result);
    }

    [Fact]
    public async Task Publish_FansOutToAllHandlers()
    {
        TestNotificationHandler1.CallCount = 0;
        TestNotificationHandler2.CallCount = 0;

        var mediator = BuildMediator();
        await mediator.Publish(new TestNotification("test"));

        Assert.Equal(1, TestNotificationHandler1.CallCount);
        Assert.Equal(1, TestNotificationHandler2.CallCount);
    }

    [Fact]
    public async Task Send_ExecutesPipelineBehavior()
    {
        TestBehavior.CallCount = 0;

        var mediator = BuildMediator();
        await mediator.Send(new PingRequest("test"));

        Assert.Equal(1, TestBehavior.CallCount);
    }
}
