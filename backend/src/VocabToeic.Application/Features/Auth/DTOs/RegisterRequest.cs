namespace VocabToeic.Application.Features.Auth.DTOs;

public class RegisterRequest
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string ConfirmPassword { get; set; } = string.Empty;
  public string FullName { get; set; } = string.Empty;
  public int TargetScore { get; set; }
  public int WordsPerDay { get; set; }

}