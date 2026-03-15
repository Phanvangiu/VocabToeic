using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.Infrastructure.Services;

public class EmailService : IEmailService
{
  private readonly string _host;
  private readonly int _port;
  private readonly string _from;
  private readonly string _password;
  private readonly string _displayName;
  private readonly string _frontendUrl;

  public EmailService(IConfiguration configuration)
  {
    _host = configuration["Email:Host"] ?? "smtp.gmail.com";
    _port = int.Parse(configuration["Email:Port"] ?? "587");
    _from = configuration["Email:From"]
        ?? throw new InvalidOperationException("Email:From is not configured.");
    _password = configuration["Email:Password"]
        ?? throw new InvalidOperationException("Email:Password is not configured.");
    _displayName = configuration["Email:DisplayName"] ?? "VocabToeic";
    _frontendUrl = configuration["Email:FrontendUrl"] ?? "http://localhost:5173";
  }

  public async Task SendVerificationEmailAsync(
      string toEmail,
      string displayName,
      string token,
      CancellationToken cancellationToken = default)
  {
    var verifyUrl = $"{_frontendUrl}/verify-email?token={token}";

    var body = $"""
            <h2>Xin chào {displayName}!</h2>
            <p>Cảm ơn bạn đã đăng ký VocabToeic.</p>
            <p>Vui lòng click vào nút bên dưới để xác thực email của bạn:</p>
            <a href="{verifyUrl}"
               style="background:#1F4E79;color:white;padding:12px 24px;
                      text-decoration:none;border-radius:6px;display:inline-block;">
              Xác thực email
            </a>
            <p>Link có hiệu lực trong <strong>24 giờ</strong>.</p>
            <p>Nếu bạn không đăng ký tài khoản này, hãy bỏ qua email này.</p>
            """;

    await SendAsync(toEmail, "Xác thực email — VocabToeic", body, cancellationToken);
  }

  public async Task SendPasswordResetEmailAsync(
      string toEmail,
      string displayName,
      string token,
      CancellationToken cancellationToken = default)
  {
    var resetUrl = $"{_frontendUrl}/reset-password?token={token}";

    var body = $"""
            <h2>Xin chào {displayName}!</h2>
            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
            <a href="{resetUrl}"
               style="background:#1F4E79;color:white;padding:12px 24px;
                      text-decoration:none;border-radius:6px;display:inline-block;">
              Đặt lại mật khẩu
            </a>
            <p>Link có hiệu lực trong <strong>1 giờ</strong>.</p>
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này.</p>
            """;

    await SendAsync(toEmail, "Đặt lại mật khẩu — VocabToeic", body, cancellationToken);
  }

  public async Task SendSetPasswordEmailAsync(
      string toEmail,
      string displayName,
      string token,
      CancellationToken cancellationToken = default)
  {
    var setPasswordUrl = $"{_frontendUrl}/set-password?token={token}";

    var body = $"""
            <h2>Xin chào {displayName}!</h2>
            <p>Bạn đang đăng nhập bằng Google. Bạn có thể đặt thêm mật khẩu để đăng nhập bằng email.</p>
            <a href="{setPasswordUrl}"
               style="background:#1F4E79;color:white;padding:12px 24px;
                      text-decoration:none;border-radius:6px;display:inline-block;">
              Đặt mật khẩu
            </a>
            <p>Link có hiệu lực trong <strong>1 giờ</strong>.</p>
            <p>Nếu bạn không yêu cầu điều này, hãy bỏ qua email này.</p>
            """;

    await SendAsync(toEmail, "Đặt mật khẩu — VocabToeic", body, cancellationToken);
  }

  // ── Private helper ────────────────────────────────────────────────────────

  private async Task SendAsync(
      string toEmail,
      string subject,
      string htmlBody,
      CancellationToken cancellationToken)
  {
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress(_displayName, _from));
    message.To.Add(MailboxAddress.Parse(toEmail));
    message.Subject = subject;
    message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

    using var client = new SmtpClient();

    // Gmail SMTP: port 587 dùng STARTTLS
    await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls, cancellationToken);
    await client.AuthenticateAsync(_from, _password, cancellationToken);
    await client.SendAsync(message, cancellationToken);
    await client.DisconnectAsync(true, cancellationToken);
  }
}