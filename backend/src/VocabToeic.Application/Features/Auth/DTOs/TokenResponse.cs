namespace VocabToeic.Application.Features.Auth.DTOs;

/// <summary>
/// Response payload returned after successful login or token refresh.
/// RawRefreshToken is used internally by the controller to set HttpOnly Cookie — NOT returned to client directly.
/// </summary>
public class TokenResponse
{
  public string AccessToken { get; set; } = string.Empty;
  public int ExpiresIn { get; set; }
  public string? RawRefreshToken { get; set; } // Internal use only — set in HttpOnly Cookie
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
  public string? AvatarUrl { get; set; }
  public int? WordsPerDay { get; set; }

}