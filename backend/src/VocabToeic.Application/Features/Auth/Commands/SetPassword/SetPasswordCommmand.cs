using FluentValidation;
using MediatR;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.Application.Features.Auth.Commands.SetPassword;

public record SetPasswordCommand(string Token, string NewPassword, string ConfirmPassword) : IRequest;

public class SetPasswordCommandValidator : AbstractValidator<SetPasswordCommand>
{
  public SetPasswordCommandValidator()
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

public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand>
{
  private readonly IUnitOfWork _uow;
  private readonly IPasswordService _passwordService;

  public SetPasswordCommandHandler(IUnitOfWork uow, IPasswordService passwordService)
  {
    _uow = uow;
    _passwordService = passwordService;
  }

  public async Task Handle(SetPasswordCommand request, CancellationToken cancellationToken)
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

    if (user.PasswordHash is not null)
      throw new Common.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                { "password", ["Tài khoản đã có mật khẩu. Dùng Forgot Password để đặt lại."] }
            });

    user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
    user.PasswordResetToken = null;
    user.PasswordResetExpiresAt = null;

    _uow.Users.Update(user);
    await _uow.SaveChangesAsync(cancellationToken);
  }
}