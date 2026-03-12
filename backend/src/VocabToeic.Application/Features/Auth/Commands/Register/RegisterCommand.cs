using MediatR;
using VocabToeic.Application.Features.Auth.DTOs;

namespace VocabToeic.Application.Features.Auth.Commands.Register;

/// <summary>
/// Command to register a new user account.
/// Returns TokenResponse with access token and user info on success.
/// </summary>
public class RegisterCommand : IRequest<TokenResponse>
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string ConfirmPassword { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  public int? TargetScore { get; set; }
}