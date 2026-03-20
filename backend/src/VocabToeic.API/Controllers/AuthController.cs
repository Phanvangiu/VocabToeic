using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Features.Auth.Commands.ForgotPassword;
using VocabToeic.Application.Features.Auth.Commands.GoogleLogin;
using VocabToeic.Application.Features.Auth.Commands.Login;
using VocabToeic.Application.Features.Auth.Commands.Logout;
using VocabToeic.Application.Features.Auth.Commands.Refresh;
using VocabToeic.Application.Features.Auth.Commands.Register;
using VocabToeic.Application.Features.Auth.Commands.ResetPassword;
using VocabToeic.Application.Features.Auth.Commands.SetPasswordFromToken;
using VocabToeic.Application.Features.Auth.Commands.VerifyEmail;
using VocabToeic.Application.Features.Auth.DTOs;

namespace VocabToeic.API.Controllers;

public class AuthController : BaseApiController
{
  private const string RefreshTokenCookieName = "refreshToken";

  /// <summary>Register a new user account. Sends verification email automatically.</summary>
  [HttpPost("register")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Register(
      [FromBody] RegisterRequest request,
      CancellationToken cancellationToken)
  {
    var command = new RegisterCommand
    {
      Email = request.Email,
      Password = request.Password,
      ConfirmPassword = request.ConfirmPassword
    };

    var result = await Mediator.Send(command, cancellationToken);
    return StatusCode(StatusCodes.Status201Created, result);
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
    SetRefreshTokenCookie(result.RawRefreshToken!);
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

  /// <summary>Login with Google ID Token from FE.</summary>
  [HttpPost("google")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> GoogleLogin(
      [FromBody] GoogleLoginRequest request,
      CancellationToken cancellationToken)
  {
    var command = new GoogleLoginCommand(request.IdToken);
    var result = await Mediator.Send(command, cancellationToken);

    SetRefreshTokenCookie(result.RawRefreshToken!);
    result.RawRefreshToken = null;

    return Ok(result);
  }

  /// <summary>Verify email — auto login on success.</summary>
  [HttpPost("verify-email")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> VerifyEmail(
      [FromBody] VerifyEmailRequest request,
      CancellationToken cancellationToken)
  {
    var result = await Mediator.Send(new VerifyEmailCommand(request.Token), cancellationToken);

    SetRefreshTokenCookie(result.RawRefreshToken!);
    result.RawRefreshToken = null;

    return Ok(result);
  }

  /// <summary>Request password reset email. Always returns 200.</summary>
  [HttpPost("forgot-password")]
  [AllowAnonymous]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> ForgotPassword(
      [FromBody] ForgotPasswordRequest request,
      CancellationToken cancellationToken)
  {
    await Mediator.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
    return Ok(new { message = "If the email exists, you will receive instructions shortly." });
  }

  /// <summary>Reset password using token from email.</summary>
  [HttpPost("reset-password")]
  [AllowAnonymous]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> ResetPassword(
      [FromBody] ResetPasswordRequest request,
      CancellationToken cancellationToken)
  {
    await Mediator.Send(
        new ResetPasswordCommand(request.Token, request.NewPassword, request.ConfirmPassword),
        cancellationToken);

    return Ok(new { message = "Password has been reset successfully." });
  }


  /// <summary>
  /// Set password for authenticated Google users who don't have a password yet.
  /// Requires Bearer token — no email token needed.
  /// </summary>
  [HttpPost("set-password/me")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> SetPasswordMe(
      [FromBody] SetPasswordFromTokenRequest request,
      CancellationToken cancellationToken)
  {
    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    await Mediator.Send(
        new SetPasswordFromTokenCommand(userId, request.NewPassword, request.ConfirmPassword),
        cancellationToken);

    return Ok(new { message = "Password set successfully. You can now log in with your email." });
  }
  // ── Private helpers ───────────────────────────────────────────────────────

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