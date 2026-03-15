namespace VocabToeic.Application.Common.Interfaces;

public record GoogleUserInfo
(
  string Sub,
  string Email,
  string DisplayName,
  string? AvatarUrl
);
public interface IGoogleAuthService
{
  Task<GoogleUserInfo> VerifyIdTokenAsync(string idToken, CancellationToken cancellationToken = default);
}