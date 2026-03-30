using MediatR;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Auth.DTOs;
using VocabToeic.Domain.Entities;
using VocabToeic.Domain.Enums;

namespace VocabToeic.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(string IdToken) : IRequest<TokenResponse>;

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, TokenResponse>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IGoogleAuthService _googleAuthService;
  private readonly IJwtService _jwtService;

  public GoogleLoginCommandHandler(
     IUnitOfWork unitOfWork,
     IGoogleAuthService googleAuthService,
     IJwtService jwtService)
  {
    _unitOfWork = unitOfWork;
    _googleAuthService = googleAuthService;
    _jwtService = jwtService;
  }
  public async Task<TokenResponse> Handle(
        GoogleLoginCommand request,
        CancellationToken cancellationToken)
  {
    var googleUser = await _googleAuthService.VerifyIdTokenAsync(request.IdToken, cancellationToken);

    var externalLogin = await _unitOfWork.ExternalLogins.GetByProviderAsync(AuthProvider.Google, googleUser.Sub, cancellationToken);

    User user;

    if (externalLogin is not null)
    {
      user = externalLogin.User;
    }
    else
    {
      user = await _unitOfWork.Users.GetByEmailAsync(
        googleUser.Email, cancellationToken) ?? await CreateNewUserAsync(googleUser, cancellationToken);

      // Link Google vào account (dù mới tạo hay có sẵn)
      var newExternalLogin = new ExternalLogin
      {
        UserId = user.Id,
        Provider = AuthProvider.Google,
        ProviderKey = googleUser.Sub,
        Email = googleUser.Email,
        DisplayName = googleUser.DisplayName,
        AvatarUrl = googleUser.AvatarUrl
      };
      await _unitOfWork.ExternalLogins.AddAsync(newExternalLogin, cancellationToken);
    }
    //  Cập nhật AvatarUrl nếu chưa có
    if (user.AvatarUrl is null && googleUser.AvatarUrl is not null)
    {
      user.AvatarUrl = googleUser.AvatarUrl;
      _unitOfWork.Users.Update(user);
    }
    // 4. Tạo token pair
    var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
    var (rawRefreshToken, hashedRefreshToken) = _jwtService.GenerateRefreshToken();

    var refreshToken = new RefreshToken
    {
      UserId = user.Id,
      Token = hashedRefreshToken,
      ExpiresAt = DateTime.UtcNow.AddDays(7)
    };

    await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new TokenResponse
    {
      AccessToken = accessToken,
      ExpiresIn = 300,
      RawRefreshToken = rawRefreshToken,
      User = new UserInfo
      {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        TargetScore = user.TargetScore,
        Streak = user.Streak,
        AvatarUrl = user.AvatarUrl,
        WordsPerDay = user.WordsPerDay
      }
    };
  }
  private async Task<User> CreateNewUserAsync(
       GoogleUserInfo googleUser,
       CancellationToken cancellationToken)
  {
    var user = new User
    {
      Email = googleUser.Email,
      DisplayName = googleUser.DisplayName ?? googleUser.Email,
      AvatarUrl = googleUser.AvatarUrl,
      // PasswordHash = null — user OAuth không có password
      Role = UserRole.User,
      EmailVerified = true,
      IsActive = true
    };

    await _unitOfWork.Users.AddAsync(user, cancellationToken);
    return user;
  }
}