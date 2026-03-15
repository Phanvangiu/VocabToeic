using MediatR;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Auth.DTOs;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponse>
{
  private readonly IUnitOfWork _uow;
  private readonly IPasswordService _passwordService;
  private readonly IJwtService _jwtService;
  private readonly IEmailService _emailService;

  public LoginCommandHandler(
      IUnitOfWork uow,
      IPasswordService passwordService,
      IJwtService jwtService,
      IEmailService emailService)
  {
    _uow = uow;
    _passwordService = passwordService;
    _jwtService = jwtService;
    _emailService = emailService;
  }

  public async Task<TokenResponse> Handle(
      LoginCommand request,
      CancellationToken cancellationToken)
  {
    var user = await _uow.Users.GetByEmailAsync(request.Email, cancellationToken);

    if (user is null || !user.IsActive)
      throw new UnauthorizedException("Invalid email or password.");

    // User đăng nhập qua Google — gửi email set password
    if (user.PasswordHash is null)
    {
      var token = GenerateToken();
      user.PasswordResetToken = token;
      user.PasswordResetExpiresAt = DateTime.UtcNow.AddHours(1);
      _uow.Users.Update(user);
      await _uow.SaveChangesAsync(cancellationToken);

      await _emailService.SendSetPasswordEmailAsync(
          user.Email, user.DisplayName, token, cancellationToken);

      throw new UnauthorizedException(
          "Tài khoản này dùng Google login. Chúng tôi đã gửi email để bạn đặt mật khẩu.");
    }

    // Chưa verify email — không cho login
    if (!user.EmailVerified)
      throw new UnauthorizedException(
          "Vui lòng xác thực email trước khi đăng nhập. Kiểm tra hộp thư của bạn.");

    var isPasswordValid = _passwordService.VerifyPassword(
        request.Password, user.PasswordHash);

    if (!isPasswordValid)
      throw new UnauthorizedException("Invalid email or password.");

    var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email);
    var (rawToken, hashedToken) = _jwtService.GenerateRefreshToken();

    var refreshToken = new RefreshToken
    {
      UserId = user.Id,
      Token = hashedToken,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
    };

    await _uow.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    await _uow.SaveChangesAsync(cancellationToken);

    return new TokenResponse
    {
      AccessToken = accessToken,
      ExpiresIn = _jwtService.GetRemainingSeconds(accessToken),
      RawRefreshToken = rawToken,
      User = new UserInfo
      {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        TargetScore = user.TargetScore,
        Streak = user.Streak,
        AvatarUrl = user.AvatarUrl
      }
    };
  }

  private static string GenerateToken()
      => Convert.ToBase64String(Guid.NewGuid().ToByteArray())
          .Replace("+", "-").Replace("/", "_").Replace("=", "");
}