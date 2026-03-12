using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace VocabToeic.Infrastructure.Services;

/// <summary>
/// Implements JWT token generation and validation using HMAC-SHA256 signing.
/// Configuration loaded from .env: JwtSettings__SecretKey, JwtSettings__Issuer, etc.
/// </summary>
public class JwtService : IJwtService
{
  private readonly string _secretKey;
  private readonly string _issuer;
  private readonly string _audience;
  private readonly int _accessTokenExpiryMinutes;

  public JwtService(IConfiguration configuration)
  {
    // Load from .env via IConfiguration
    _secretKey = configuration["JwtSettings:SecretKey"]
        ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");
    _issuer = configuration["JwtSettings:Issuer"] ?? "VocabToeic";
    _audience = configuration["JwtSettings:Audience"] ?? "VocabToeicClient";
    _accessTokenExpiryMinutes = int.Parse(
        configuration["JwtSettings:AccessTokenExpiryMinutes"] ?? "5");
  }

  public string GenerateAccessToken(Guid userId, string email)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            // JTI = unique ID for each token, used for Redis blacklist
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

    var token = new JwtSecurityToken(
        issuer: _issuer,
        audience: _audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public (string rawToken, string hashedToken) GenerateRefreshToken()
  {
    var rawBytes = RandomNumberGenerator.GetBytes(64);
    var rawToken = Convert.ToBase64String(rawBytes);

    var hashedToken = HashToken(rawToken);

    return (rawToken, hashedToken);
  }

  public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

    var validationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = false, // Allow expired tokens
      ValidateIssuerSigningKey = true,
      ValidIssuer = _issuer,
      ValidAudience = _audience,
      IssuerSigningKey = key
    };

    var handler = new JwtSecurityTokenHandler();
    var principal = handler.ValidateToken(token, validationParameters, out _);
    return principal;
  }

  public string GetJtiFromToken(string token)
  {
    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    return jwtToken.Claims
        .First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
  }

  public int GetRemainingSeconds(string token)
  {
    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    var expiry = jwtToken.ValidTo;
    var remaining = (int)(expiry - DateTime.UtcNow).TotalSeconds;
    // Return 0 if already expired
    return Math.Max(0, remaining);
  }

  // Hash refresh token using SHA256 before storing in DB
  private static string HashToken(string token)
  {
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
    return Convert.ToBase64String(bytes);
  }
}