using MediatR;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IEmailService _emailService;

  public ForgotPasswordCommandHandler(
      IUnitOfWork unitOfWork,
      IEmailService emailService)
  {
    _unitOfWork = unitOfWork;
    _emailService = emailService;
  }

  public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
  {
    var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

    // Không báo lỗi nếu email không tồn tại — tránh email enumeration attack
    // Attacker không biết email nào đã đăng ký
    if (user is null || !user.IsActive) return;

    // User OAuth không có password — gửi email set password thay vì reset
    if (user.PasswordHash is null)
    {
      var setPasswordToken = GenerateToken();
      user.PasswordResetToken = setPasswordToken;
      user.PasswordResetExpiresAt = DateTime.UtcNow.AddHours(1);

      _unitOfWork.Users.Update(user);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      await _emailService.SendSetPasswordEmailAsync(
          user.Email, user.DisplayName, setPasswordToken, cancellationToken);
      return;
    }

    var token = GenerateToken();
    user.PasswordResetToken = token;
    user.PasswordResetExpiresAt = DateTime.UtcNow.AddHours(1);

    _unitOfWork.Users.Update(user);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    await _emailService.SendPasswordResetEmailAsync(
        user.Email, user.DisplayName, token, cancellationToken);
  }

  private static string GenerateToken()
      => Convert.ToBase64String(Guid.NewGuid().ToByteArray())
          .Replace("+", "-").Replace("/", "_").Replace("=", "");
}