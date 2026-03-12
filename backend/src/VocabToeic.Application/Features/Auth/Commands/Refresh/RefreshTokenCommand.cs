using MediatR;
using VocabToeic.Application.Features.Auth.DTOs;

namespace VocabToeic.Application.Features.Auth.Commands.Refresh;

/// <summary>
/// Command to refresh an expired access token using a valid refresh token.
/// Refresh token is read from HttpOnly Cookie by the controller before dispatching this command.
/// </summary>
public class RefreshTokenCommand : IRequest<TokenResponse>
{
  public string RawRefreshToken { get; set; } = string.Empty;
}