using MediatR;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Auth.DTOs;
using VocabToeic.Domain.Entities;
using Microsoft.Extensions.Logging; // ← thêm using

namespace VocabToeic.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponse>
{
  private readonly IUnitOfWork _uow;
  private readonly IPasswordService _passwordService;
  private readonly IJwtService _jwtService;
  private readonly IEmailService _emailService;
  private readonly ILogger<LoginCommandHandler> _logger; // ← thêm field

  public LoginCommandHandler(
      IUnitOfWork uow,
      IPasswordService passwordService,
      IJwtService jwtService,
      IEmailService emailService,
      ILogger<LoginCommandHandler> logger) // ← thêm tham số
  {
    _uow = uow;
    _passwordService = passwordService;
    _jwtService = jwtService;
    _emailService = emailService;
    _logger = logger; // ← thêm assign
  }

  public async Task<TokenResponse> Handle(
      LoginCommand request,
      CancellationToken cancellationToken)
  {
    var total = System.Diagnostics.Stopwatch.StartNew(); // ← thêm
    var sw = System.Diagnostics.Stopwatch.StartNew();    // ← thêm

    var user = await _uow.Users.GetByEmailAsync(request.Email, cancellationToken);
    _logger.LogInformation("[PERF:Login] GetByEmail: {Ms}ms", sw.ElapsedMilliseconds); // ← thêm

    if (user is null || !user.IsActive)
      throw new UnauthorizedException("Invalid email or password.");

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

    if (!user.EmailVerified)
      throw new UnauthorizedException(
          "Vui lòng xác thực email trước khi đăng nhập. Kiểm tra hộp thư của bạn.");

    sw.Restart(); // ← thêm
    var isPasswordValid = _passwordService.VerifyPassword(
        request.Password, user.PasswordHash);
    _logger.LogInformation("[PERF:Login] VerifyPassword: {Ms}ms", sw.ElapsedMilliseconds); // ← thêm

    if (!isPasswordValid)
      throw new UnauthorizedException("Invalid email or password.");

    sw.Restart(); // ← thêm
    var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email);
    var (rawToken, hashedToken) = _jwtService.GenerateRefreshToken();
    _logger.LogInformation("[PERF:Login] GenerateTokens: {Ms}ms", sw.ElapsedMilliseconds); // ← thêm

    sw.Restart(); // ← thêm
    var refreshToken = new RefreshToken
    {
      UserId = user.Id,
      Token = hashedToken,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
    };

    await _uow.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    await _uow.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("[PERF:Login] SaveRefreshToken: {Ms}ms", sw.ElapsedMilliseconds); // ← thêm

    _logger.LogInformation("[PERF:Login] Total: {Ms}ms", total.ElapsedMilliseconds); // ← thêm

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
        AvatarUrl = user.AvatarUrl,
        WordsPerDay = user.WordsPerDay
      }
    };
  }

  private static string GenerateToken()
      => Convert.ToBase64String(Guid.NewGuid().ToByteArray())
          .Replace("+", "-").Replace("/", "_").Replace("=", "");
}