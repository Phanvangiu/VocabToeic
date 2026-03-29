using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.API.Services;

public class CurrentUserService : ICurrentUserService
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CurrentUserService(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  private ClaimsPrincipal User =>
      _httpContextAccessor.HttpContext?.User
      ?? throw new UnauthorizedException("No HTTP context.");

  public Guid UserId
  {
    get
    {
      var value = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
               ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new UnauthorizedException("UserId claim not found.");
      return Guid.Parse(value);
    }
  }

  public string Email =>
      User.FindFirstValue(JwtRegisteredClaimNames.Email)
      ?? User.FindFirstValue(ClaimTypes.Email)
      ?? throw new UnauthorizedException("Email claim not found.");

  public bool IsAuthenticated =>
      User.Identity?.IsAuthenticated ?? false;
}