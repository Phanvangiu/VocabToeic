namespace VocabToeic.Application.Features.Auth.DTOs;

/// <summary>
/// Request payload for user registration.
/// </summary>
public class RegisterRequest
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string ConfirmPassword { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  public int? TargetScore { get; set; }
}