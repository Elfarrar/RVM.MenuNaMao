namespace RVM.MenuNaMao.API.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string Header = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey(Header))
            context.Request.Headers[Header] = Guid.NewGuid().ToString();

        var correlationId = context.Request.Headers[Header].ToString();
        context.Response.Headers[Header] = correlationId;

        await next(context);
    }
}
