namespace VocabToeic.Application.Features.Auth.DTOs;

/// <summary>
/// Request payload for user login.
/// </summary>
public class LoginRequest
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}