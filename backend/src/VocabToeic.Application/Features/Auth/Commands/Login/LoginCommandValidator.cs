using FluentValidation;

namespace VocabToeic.Application.Features.Auth.Commands.Login;

/// <summary>
/// Validates LoginCommand input before reaching the handler.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
  public LoginCommandValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required.")
        .EmailAddress().WithMessage("Invalid email format.");

    RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.");
  }
}