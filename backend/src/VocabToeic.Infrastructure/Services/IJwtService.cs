using System.Security.Claims;

namespace VocabToeic.Infrastructure.Services;

/// <summary>
/// Provides JWT token generation and validation operations.
/// </summary>
public interface IJwtService
{
  /// <summary>
  /// Generates a signed JWT access token for the given user.
  /// Token expires in AccessTokenExpiryMinutes (configured in .env).
  /// </summary>
  /// <param name="userId">User's unique identifier.</param>
  /// <param name="email">User's email address.</param>
  /// <returns>Signed JWT token string.</returns>
  string GenerateAccessToken(Guid userId, string email);

  /// <summary>
  /// Generates a cryptographically secure refresh token.
  /// Returns raw token (to send to client) and hashed token (to store in DB).
  /// </summary>
  /// <returns>Tuple of (rawToken, hashedToken).</returns>
  (string rawToken, string hashedToken) GenerateRefreshToken();

  /// <summary>
  /// Extracts all claims from a JWT token without validating expiry.
  /// Used when refreshing tokens to read claims from expired access tokens.
  /// </summary>
  /// <param name="token">JWT token string.</param>
  /// <returns>ClaimsPrincipal containing all claims.</returns>
  ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

  /// <summary>
  /// Extracts the JTI (JWT ID) claim from a token.
  /// Used for Redis blacklist on logout.
  /// </summary>
  /// <param name="token">JWT token string.</param>
  /// <returns>JTI claim value.</returns>
  string GetJtiFromToken(string token);

  /// <summary>
  /// Returns the remaining lifetime of a token in seconds.
  /// Used to set Redis blacklist TTL on logout.
  /// </summary>
  /// <param name="token">JWT token string.</param>
  /// <returns>Remaining seconds until token expiry.</returns>
  int GetRemainingSeconds(string token);
}