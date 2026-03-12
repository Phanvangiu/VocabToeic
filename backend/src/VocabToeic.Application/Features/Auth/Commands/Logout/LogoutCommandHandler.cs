using MediatR;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Handles user logout — revokes refresh token in DB and blacklists access token in Redis.
/// After logout, both tokens are immediately invalidated.
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
  private readonly IUnitOfWork _uow;
  private readonly IJwtService _jwtService;
  private readonly IRedisService _redisService;

  public LogoutCommandHandler(
      IUnitOfWork uow,
      IJwtService jwtService,
      IRedisService redisService)
  {
    _uow = uow;
    _jwtService = jwtService;
    _redisService = redisService;
  }

  public async Task Handle(
      LogoutCommand request,
      CancellationToken cancellationToken)
  {
    // Blacklist access token in Redis — TTL = remaining lifetime
    var jti = _jwtService.GetJtiFromToken(request.AccessToken);
    var remainingSeconds = _jwtService.GetRemainingSeconds(request.AccessToken);

    if (remainingSeconds > 0)
      await _redisService.SetAsync($"blacklist:{jti}", "1", remainingSeconds);

    // Revoke refresh token in DB
    var hashedToken = HashToken(request.RawRefreshToken);
    var refreshToken = await _uow.RefreshTokens
        .GetByHashedTokenAsync(hashedToken, cancellationToken);

    if (refreshToken != null && refreshToken.RevokedAt == null)
    {
      refreshToken.RevokedAt = DateTime.UtcNow;
      refreshToken.RevokedReason = "Logout";
      _uow.RefreshTokens.Update(refreshToken);
      await _uow.SaveChangesAsync(cancellationToken);
    }
  }

  private static string HashToken(string token)
  {
    var bytes = System.Security.Cryptography.SHA256.HashData(
        System.Text.Encoding.UTF8.GetBytes(token));
    return Convert.ToBase64String(bytes);
  }
}