using System.Diagnostics;
using RVM.MenuNaMao.Application.Mediator;
using Microsoft.Extensions.Logging;

namespace RVM.MenuNaMao.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Handling {RequestName}", requestName);

        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
        return response;
    }
}
