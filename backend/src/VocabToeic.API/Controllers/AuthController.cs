using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Features.Auth.Commands.Login;
using VocabToeic.Application.Features.Auth.Commands.Logout;
using VocabToeic.Application.Features.Auth.Commands.Refresh;
using VocabToeic.Application.Features.Auth.Commands.Register;
using VocabToeic.Application.Features.Auth.DTOs;

namespace VocabToeic.API.Controllers;

/// <summary>
/// Handles authentication endpoints: register, login, refresh token, logout.
/// Refresh token is managed via HttpOnly Cookie — never exposed in response body.
/// </summary>
public class AuthController : BaseApiController
{
  private const string RefreshTokenCookieName = "refreshToken";

  /// <summary>Register a new user account.</summary>
  [HttpPost("register")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Register(
      [FromBody] RegisterRequest request,
      CancellationToken cancellationToken)
  {
    var command = new RegisterCommand
    {
      Email = request.Email,
      Password = request.Password,
      ConfirmPassword = request.ConfirmPassword,
      DisplayName = request.DisplayName,
      TargetScore = request.TargetScore
    };

    var result = await Mediator.Send(command, cancellationToken);
    return CreatedAtAction(nameof(Register), result);
  }

  /// <summary>Authenticate user and issue tokens.</summary>
  [HttpPost("login")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> Login(
      [FromBody] LoginRequest request,
      CancellationToken cancellationToken)
  {
    var command = new LoginCommand
    {
      Email = request.Email,
      Password = request.Password
    };

    var result = await Mediator.Send(command, cancellationToken);

    // Set refresh token in HttpOnly Cookie
    SetRefreshTokenCookie(result.RawRefreshToken!);

    // Remove raw refresh token from response body
    result.RawRefreshToken = null;

    return Ok(result);
  }

  /// <summary>Refresh expired access token using refresh token from Cookie.</summary>
  [HttpPost("refresh")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
  {
    var rawRefreshToken = Request.Cookies[RefreshTokenCookieName];

    if (string.IsNullOrEmpty(rawRefreshToken))
      throw new UnauthorizedException("Refresh token not found.");

    var command = new RefreshTokenCommand { RawRefreshToken = rawRefreshToken };
    var result = await Mediator.Send(command, cancellationToken);

    // Rotate cookie
    SetRefreshTokenCookie(result.RawRefreshToken!);
    result.RawRefreshToken = null;

    return Ok(result);
  }

  /// <summary>Logout user — revokes tokens and clears Cookie.</summary>
  [HttpPost("logout")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> Logout(CancellationToken cancellationToken)
  {
    var accessToken = Request.Headers.Authorization
        .ToString().Replace("Bearer ", "");

    var rawRefreshToken = Request.Cookies[RefreshTokenCookieName] ?? string.Empty;

    var command = new LogoutCommand
    {
      AccessToken = accessToken,
      RawRefreshToken = rawRefreshToken
    };

    await Mediator.Send(command, cancellationToken);
    ClearRefreshTokenCookie();

    return Ok(new { message = "Logged out successfully." });
  }

  // ── Private helpers ────────────────────────────────────

  private void SetRefreshTokenCookie(string rawToken)
  {
    Response.Cookies.Append(RefreshTokenCookieName, rawToken, new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = DateTimeOffset.UtcNow.AddDays(7)
    });
  }

  private void ClearRefreshTokenCookie()
  {
    Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict
    });
  }
}