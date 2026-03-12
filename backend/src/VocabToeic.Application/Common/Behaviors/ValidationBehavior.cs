using FluentValidation;
using MediatR;
namespace VocabToeic.Application.Common.Behaviors;
/// <summary>
/// MediatR pipeline behavior that automatically validates requests before reaching handlers.
/// Eliminates the need to manually call validation in each handler.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull

{
  private readonly IEnumerable<IValidator<TRequest>> _validators;
  // Inject all validators for TRequest — FluentValidation finds them automatically

  public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
  {
    _validators = validators;
  }
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (!_validators.Any())
    {
      return await next();
    }
    // Run all validators in parallel
    var context = new ValidationContext<TRequest>(request);
    var validationResults = await Task.WhenAll(
      _validators.Select(v => v.ValidateAsync(context, cancellationToken))
    );

    var failures = validationResults
      .SelectMany(r => r.Errors)
      .Where(f => f != null)
      .GroupBy(f => f.PropertyName)
      .ToDictionary(
        g => g.Key,
        g => g.Select(f => f.ErrorMessage).ToArray()
      );

    if (failures.Any())
    {
      throw new Exceptions.ValidationException(failures);
    }
    return await next();
  }
}