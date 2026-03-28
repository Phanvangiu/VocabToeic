using FluentValidation;

namespace VocabToeic.Application.Features.Auth.Commands.Register;

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

    RuleFor(x => x.FullName)
        .NotEmpty().WithMessage("Full name is required.")
        .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

    RuleFor(x => x.TargetScore)
        .InclusiveBetween(10, 990).WithMessage("Target score must be between 10 and 990.");

    RuleFor(x => x.WordsPerDay)
        .GreaterThan(0).WithMessage("Words per day must be greater than 0.")
        .LessThanOrEqualTo(100).WithMessage("Words per day must not exceed 100.");
  }
}