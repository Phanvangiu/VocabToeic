using FluentValidation;

namespace VocabToeic.Application.Features.Words.Commands.CreateWord;

public class CreateWordCommandValidator : AbstractValidator<CreateWordCommand>
{
  public CreateWordCommandValidator()
  {
    RuleFor(x => x.Term)
        .NotEmpty().WithMessage("Term is required.")
        .MaximumLength(100).WithMessage("Term must not exceed 100 characters.")
        .Must(t => t == t.Trim()).WithMessage("Term must not have leading or trailing whitespace.");

    RuleFor(x => x.Topic)
        .NotEmpty().WithMessage("Topic is required.")
        .MaximumLength(50).WithMessage("Topic must not exceed 50 characters.");

    RuleFor(x => x.Level)
        .InclusiveBetween(1, 3).WithMessage("Level must be 1, 2, or 3.");

    RuleFor(x => x.Definitions)
        .NotEmpty().WithMessage("At least one definition is required.");

    RuleForEach(x => x.Definitions).ChildRules(d =>
    {
      d.RuleFor(x => x.DefinitionEn)
              .NotEmpty().WithMessage("English definition is required.");
      d.RuleFor(x => x.DefinitionVi)
              .NotEmpty().WithMessage("Vietnamese definition is required.");
    });
  }
}