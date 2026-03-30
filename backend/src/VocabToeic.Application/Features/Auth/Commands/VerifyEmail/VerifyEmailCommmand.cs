using MediatR;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Auth.DTOs;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Features.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string Token) : IRequest<TokenResponse>;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, TokenResponse>
{
  private readonly IUnitOfWork _uow;
  private readonly IJwtService _jwtService;

  public VerifyEmailCommandHandler(IUnitOfWork uow, IJwtService jwtService)
  {
    _uow = uow;
    _jwtService = jwtService;
  }

  public async Task<TokenResponse> Handle(
      VerifyEmailCommand request,
      CancellationToken cancellationToken)
  {
    var user = await _uow.Users.FirstOrDefaultAsync(
        u => u.EmailVerificationToken == request.Token, cancellationToken)
        ?? throw new ValidationException(new Dictionary<string, string[]>
        {
                { "token", ["Token không hợp lệ."] }
        });

    if (user.EmailVerificationExpiresAt < DateTime.UtcNow)
      throw new ValidationException(new Dictionary<string, string[]>
            {
                { "token", ["Token đã hết hạn. Vui lòng yêu cầu gửi lại."] }
            });

    if (user.EmailVerified)
      throw new ValidationException(new Dictionary<string, string[]>
            {
                { "email", ["Email đã được xác thực trước đó."] }
            });

    user.EmailVerified = true;
    user.EmailVerificationToken = null;
    user.EmailVerificationExpiresAt = null;
    _uow.Users.Update(user);

    var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
    var (rawToken, hashedToken) = _jwtService.GenerateRefreshToken();

    var refreshToken = new RefreshToken
    {
      UserId = user.Id,
      Token = hashedToken,
      ExpiresAt = DateTime.UtcNow.AddDays(7)
    };

    await _uow.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    await _uow.SaveChangesAsync(cancellationToken);

    return new TokenResponse
    {
      AccessToken = accessToken,
      ExpiresIn = _jwtService.GetRemainingSeconds(accessToken),
      RawRefreshToken = rawToken,
      User = new UserInfo
      {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        TargetScore = user.TargetScore,
        Streak = user.Streak,
        AvatarUrl = user.AvatarUrl
      }
    };
  }
}