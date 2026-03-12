using System.Security.Claims;

namespace VocabToeic.Application.Common.Interfaces;

/// <summary>
/// Provides JWT token generation and validation operations.
/// </summary>
public interface IJwtService
{
  string GenerateAccessToken(Guid userId, string email);
  (string rawToken, string hashedToken) GenerateRefreshToken();
  ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
  string GetJtiFromToken(string token);
  int GetRemainingSeconds(string token);
}