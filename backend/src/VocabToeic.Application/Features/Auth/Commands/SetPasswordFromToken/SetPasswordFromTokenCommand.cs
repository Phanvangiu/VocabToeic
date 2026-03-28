using FluentValidation;
using MediatR;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.Application.Features.Auth.Commands.SetPasswordFromToken;

public record SetPasswordFromTokenCommand(
    Guid UserId,
    string NewPassword,
    string ConfirmPassword) : IRequest;

public class SetPasswordFromTokenCommandValidator : AbstractValidator<SetPasswordFromTokenCommand>
{
  public SetPasswordFromTokenCommandValidator()
  {
    RuleFor(x => x.NewPassword)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
        .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
        .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
        .Matches("[0-9]").WithMessage("Password must contain at least one number.");

    RuleFor(x => x.ConfirmPassword)
        .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
  }
}

public class SetPasswordFromTokenCommandHandler : IRequestHandler<SetPasswordFromTokenCommand>
{
  private readonly IUnitOfWork _uow;
  private readonly IPasswordService _passwordService;

  public SetPasswordFromTokenCommandHandler(IUnitOfWork uow, IPasswordService passwordService)
  {
    _uow = uow;
    _passwordService = passwordService;
  }

  public async Task Handle(SetPasswordFromTokenCommand request, CancellationToken cancellationToken)
  {
    var user = await _uow.Users.GetByIdAsync(request.UserId, cancellationToken)
        ?? throw new NotFoundException("User", request.UserId);

    // Only allow setting password if user doesn't have one yet
    if (user.PasswordHash is not null)
      throw new Common.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                { "password", ["Account already has a password. Use Forgot Password to reset it."] }
            });

    user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
    user.EmailVerified = true;
    _uow.Users.Update(user);
    await _uow.SaveChangesAsync(cancellationToken);
  }
}