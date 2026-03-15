using FluentValidation;
using MediatR;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword, string ConfirmPassword) : IRequest;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
  public ResetPasswordCommandValidator()
  {
    RuleFor(x => x.Token).NotEmpty();
    RuleFor(x => x.NewPassword)
        .NotEmpty()
        .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
        .Matches("[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ hoa.")
        .Matches("[0-9]").WithMessage("Mật khẩu phải có ít nhất 1 chữ số.");
    RuleFor(x => x.ConfirmPassword)
        .Equal(x => x.NewPassword).WithMessage("Mật khẩu xác nhận không khớp.");
  }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
  private readonly IUnitOfWork _uow;
  private readonly IPasswordService _passwordService;

  public ResetPasswordCommandHandler(IUnitOfWork uow, IPasswordService passwordService)
  {
    _uow = uow;
    _passwordService = passwordService;
  }

  public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
  {
    var user = await _uow.Users.FirstOrDefaultAsync(
        u => u.PasswordResetToken == request.Token, cancellationToken)
        ?? throw new Common.Exceptions.ValidationException(new Dictionary<string, string[]>
        {
                { "token", ["Token không hợp lệ."] }
        });

    if (user.PasswordResetExpiresAt < DateTime.UtcNow)
      throw new Common.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                { "token", ["Token đã hết hạn. Vui lòng yêu cầu lại."] }
            });

    user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
    user.PasswordResetToken = null;
    user.PasswordResetExpiresAt = null;

    await _uow.RefreshTokens.RevokeAllUserTokensAsync(user.Id, "Password reset", cancellationToken);

    _uow.Users.Update(user);
    await _uow.SaveChangesAsync(cancellationToken);
  }
}