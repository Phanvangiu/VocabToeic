using MediatR;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Auth.DTOs;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Features.Auth.Commands.Refresh;

/// <summary>
/// Handles refresh token rotation — validates old token, revokes it, issues new access and refresh tokens.
/// Detects token reuse attack: if a revoked token is used again, all user tokens are revoked immediately.
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
  private readonly IUnitOfWork _uow;
  private readonly IJwtService _jwtService;

  private readonly IRedisService _redisService;

  public RefreshTokenCommandHandler(
      IUnitOfWork uow,
      IJwtService jwtService,
      IRedisService redisService)
  {
    _uow = uow;
    _jwtService = jwtService;
    _redisService = redisService;
  }

  public async Task<TokenResponse> Handle(
      RefreshTokenCommand request,
      CancellationToken cancellationToken)
  {
    // Hash the raw token to compare with DB
    var hashedToken = HashToken(request.RawRefreshToken);

    var refreshToken = await _uow.RefreshTokens
        .GetByHashedTokenAsync(hashedToken, cancellationToken);

    if (refreshToken is null)
      throw new UnauthorizedException("Invalid refresh token.");

    // Detect token reuse attack — token already revoked
    if (refreshToken.RevokedAt != null)
    {
      // Revoke all tokens for this user immediately
      await _uow.RefreshTokens.RevokeAllUserTokensAsync(
          refreshToken.UserId,
          "Token reuse attack detected.",
          cancellationToken);
      await _uow.SaveChangesAsync(cancellationToken);
      throw new UnauthorizedException("Token reuse detected. All sessions have been revoked.");
    }

    // Check token expiry
    if (refreshToken.ExpiresAt < DateTime.UtcNow)
      throw new UnauthorizedException("Refresh token has expired.");

    // Revoke old token — Rotation
    refreshToken.RevokedAt = DateTime.UtcNow;
    refreshToken.RevokedReason = "Rotated";
    _uow.RefreshTokens.Update(refreshToken);

    if (!string.IsNullOrEmpty(request.OldAccessToken))
    {
      var jti = _jwtService.GetJtiFromToken(request.OldAccessToken);
      var remaining = _jwtService.GetRemainingSeconds(request.OldAccessToken);
      if (remaining > 0)
        await _redisService.SetAsync($"blacklist:{jti}", "revoked", remaining);
    }

    // Generate new tokens
    var newAccessToken = _jwtService.GenerateAccessToken(
        refreshToken.UserId, refreshToken.User.Email);

    var (rawToken, hashedNewToken) = _jwtService.GenerateRefreshToken();

    var newRefreshToken = new RefreshToken
    {
      UserId = refreshToken.UserId,
      Token = hashedNewToken,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
    };

    await _uow.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
    await _uow.SaveChangesAsync(cancellationToken);

    return new TokenResponse
    {
      AccessToken = newAccessToken,
      ExpiresIn = _jwtService.GetRemainingSeconds(newAccessToken),
      RawRefreshToken = rawToken,
      User = new UserInfo
      {
        Id = refreshToken.User.Id,
        Email = refreshToken.User.Email,
        DisplayName = refreshToken.User.DisplayName,
        TargetScore = refreshToken.User.TargetScore,
        Streak = refreshToken.User.Streak
      }
    };
  }

  // Hash raw refresh token using SHA256 to compare with stored hash
  private static string HashToken(string token)
  {
    var bytes = System.Security.Cryptography.SHA256.HashData(
        System.Text.Encoding.UTF8.GetBytes(token));
    return Convert.ToBase64String(bytes);
  }
}