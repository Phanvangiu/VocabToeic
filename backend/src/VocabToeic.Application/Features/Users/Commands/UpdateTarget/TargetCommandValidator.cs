using FluentValidation;

namespace VocabToeic.Application.Features.Users.Commands.UpdateTarget;

public class TargetCommandValidator : AbstractValidator<TargetCommand>
{
  public TargetCommandValidator()
  {
    RuleFor(x => x.TargetScore)
        .InclusiveBetween(10, 990).WithMessage("Target score must be between 10 and 990.");

    RuleFor(x => x.WordsPerDay)
        .GreaterThan(0).WithMessage("Words per day must be greater than 0.")
        .LessThanOrEqualTo(100).WithMessage("Words per day must not exceed 100.");
  }
}