namespace VocabToeic.Application.Common.Interfaces;

public interface IEmailService
{
  Task SendVerificationEmailAsync(
      string toEmail,
      string displayName,
      string token,
      CancellationToken cancellationToken = default);

  Task SendPasswordResetEmailAsync(
      string toEmail,
      string displayName,
      string token,
      CancellationToken cancellationToken = default);

  Task SendSetPasswordEmailAsync(
      string toEmail,
      string displayName,
      string token,
      CancellationToken cancellationToken = default);
}