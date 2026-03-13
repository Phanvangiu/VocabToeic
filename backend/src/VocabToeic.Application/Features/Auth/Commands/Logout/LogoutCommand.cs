using MediatR;

namespace VocabToeic.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Command to logout a user — revokes refresh token and blacklists access token in Redis.
/// AccessToken and RawRefreshToken are extracted from request by the controller.
/// </summary>
public class LogoutCommand : IRequest
{
  public string AccessToken { get; set; } = string.Empty;
  public string RawRefreshToken { get; set; } = string.Empty;
}