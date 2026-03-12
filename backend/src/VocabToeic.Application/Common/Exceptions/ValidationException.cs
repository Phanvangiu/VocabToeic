namespace VocabToeic.Application.Common.Exceptions;

/// <summary>
/// Represents validation errors returned when user input fails validation rules.
/// Usage: throw new ValidationException(failures) inside ValidationBehavior.
/// </summary>
public class ValidationException : Exception
{
  public IDictionary<string, string[]> Errors { get; }

  public ValidationException(IDictionary<string, string[]> errors)
      : base("One or more validation errors occurred.")
  {
    Errors = errors;
  }
}