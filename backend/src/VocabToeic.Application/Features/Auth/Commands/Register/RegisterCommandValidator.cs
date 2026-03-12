using FluentValidation;

namespace VocabToeic.Application.Features.Auth.Commands.Register;

/// <summary>
/// Validates RegisterCommand input before reaching the handler.
/// Automatically executed by ValidationBehavior in MediatR pipeline.
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
  public RegisterCommandValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required.")
        .EmailAddress().WithMessage("Invalid email format.")
        .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

    RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
        .MaximumLength(100).WithMessage("Password must not exceed 100 characters.")
        .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
        .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
        .Matches("[0-9]").WithMessage("Password must contain at least one number.");

    RuleFor(x => x.ConfirmPassword)
        .NotEmpty().WithMessage("Confirm password is required.")
        .Equal(x => x.Password).WithMessage("Passwords do not match.");

    RuleFor(x => x.DisplayName)
        .NotEmpty().WithMessage("Display name is required.")
        .MinimumLength(2).WithMessage("Display name must be at least 2 characters.")
        .MaximumLength(100).WithMessage("Display name must not exceed 100 characters.");

    RuleFor(x => x.TargetScore)
        .InclusiveBetween(10, 990).WithMessage("Target score must be between 10 and 990.")
        .When(x => x.TargetScore.HasValue);
  }
}