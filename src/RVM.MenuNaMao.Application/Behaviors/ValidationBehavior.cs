using RVM.MenuNaMao.Application.Mediator;

namespace RVM.MenuNaMao.Application.Behaviors;

public interface IValidator<in T>
{
    Task<List<string>> ValidateAsync(T instance, CancellationToken ct = default);
}

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
    {
        var errors = new List<string>();

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(request, ct);
            errors.AddRange(result);
        }

        if (errors.Count > 0)
            throw new ValidationException(errors);

        return await next();
    }
}

public class ValidationException(List<string> errors) : Exception("Validation failed")
{
    public List<string> Errors { get; } = errors;
}
