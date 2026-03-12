using MediatR;
using VocabToeic.Application.Features.Auth.DTOs;

namespace VocabToeic.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user and issue JWT tokens.
/// Returns TokenResponse with access token and user info on success.
/// </summary>
public class LoginCommand : IRequest<TokenResponse>
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}