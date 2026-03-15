using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.Infrastructure.Services;

public class GoogleAuthService : IGoogleAuthService
{
  private readonly string _clientId;
  public GoogleAuthService(IConfiguration configuration)
  {
    _clientId = configuration["Google:ClientId"] ??
      throw new InvalidOperationException("Google:ClientId is not configured.");
  }

  public async Task<GoogleUserInfo> VerifyIdTokenAsync(string idToken, CancellationToken cancellationToken = default)
  {
    try
    {
      // Google.Apis.Auth tự verify:
      // - Signature (chữ ký của Google)
      // - Expiry (token chưa hết hạn)
      // - Audience (ClientId khớp)
      var settings = new GoogleJsonWebSignature.ValidationSettings
      {
        Audience = [_clientId]
      };

      var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

      return new GoogleUserInfo(
        Sub: payload.Subject,
        Email: payload.Email,
        DisplayName: payload.Name,
        AvatarUrl: payload.Picture
      );
    }
    catch (InvalidJwtException ex)
    {
      throw new UnauthorizedException($"Invalid Google token: {ex.Message}");
    }
  }
}