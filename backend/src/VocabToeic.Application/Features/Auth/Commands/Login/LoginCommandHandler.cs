using MediatR;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Auth.DTOs;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handles user authentication — verifies credentials, issues JWT access token and refresh token.
/// Refresh token is returned as raw value to be set in HttpOnly Cookie by the controller.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponse>
{
  private readonly IUnitOfWork _uow;
  private readonly IPasswordService _passwordService;
  private readonly IJwtService _jwtService;

  public LoginCommandHandler(
      IUnitOfWork uow,
      IPasswordService passwordService,
      IJwtService jwtService)
  {
    _uow = uow;
    _passwordService = passwordService;
    _jwtService = jwtService;
  }

  public async Task<TokenResponse> Handle(
      LoginCommand request,
      CancellationToken cancellationToken)
  {
    // Find user by email
    var user = await _uow.Users.GetByEmailAsync(
        request.Email, cancellationToken);

    // Use generic error message to prevent user enumeration attack
    if (user is null || !user.IsActive)
      throw new UnauthorizedException("Invalid email or password.");
    if (user.PasswordHash is null)
    {
      // Todo: Gửi email "Set Password" để user có thể đăng nhập bằng cả 2 cách
      // Cần implement: SetPasswordCommand + MailKit email service
      throw new UnauthorizedException("This account uses Google login. Please sign in with Google.");
    }
    // Verify password
    var isPasswordValid = _passwordService.VerifyPassword(
        request.Password, user.PasswordHash);

    if (!isPasswordValid)
      throw new UnauthorizedException("Invalid email or password.");

    // Generate access token
    var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email);

    // Generate refresh token — store hashed in DB, return raw to client
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
      RawRefreshToken = rawToken, // Controller will set this in HttpOnly Cookie
      User = new UserInfo
      {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        TargetScore = user.TargetScore,
        Streak = user.Streak
      }
    };
  }
}