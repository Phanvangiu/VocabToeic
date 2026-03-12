namespace VocabToeic.Application.Features.Auth.DTOs;

/// <summary>
/// Response payload returned after successful login or token refresh.
/// Refresh token is NOT included here — it is sent via HttpOnly Cookie.
/// </summary>
public class TokenResponse
{
  public string AccessToken { get; set; } = string.Empty;
  public int ExpiresIn { get; set; } // Access token lifetime in seconds
  public UserInfo User { get; set; } = null!;
}

/// <summary>
/// Basic user information returned alongside the token.
/// Excludes sensitive fields like PasswordHash.
/// </summary>
public class UserInfo
{
  public Guid Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  public int? TargetScore { get; set; }
  public int Streak { get; set; }
}