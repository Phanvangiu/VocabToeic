using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.Infrastructure.Services;

public class EmailService : IEmailService
{
  private readonly string _apiKey;
  private readonly string _from;
  private readonly string _displayName;
  private readonly string _frontendUrl;
  private readonly HttpClient _httpClient;

  public EmailService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
  {
    _apiKey = configuration["Email:ApiKey"]
        ?? throw new InvalidOperationException("Email:ApiKey is not configured.");
    _from = configuration["Email:From"]
        ?? throw new InvalidOperationException("Email:From is not configured.");
    _displayName = configuration["Email:DisplayName"] ?? "VocabToeic";
    _frontendUrl = configuration["Email:FrontendUrl"] ?? "http://localhost:5173";
    _httpClient = httpClientFactory.CreateClient();
  }

  public async Task SendVerificationEmailAsync(
      string toEmail, string displayName, string token,
      CancellationToken cancellationToken = default)
  {
    var verifyUrl = $"{_frontendUrl}/verify-email?token={token}";
    var body = $"""
      <h2>Xin chào {displayName}!</h2>
      <p>Cảm ơn bạn đã đăng ký VocabToeic.</p>
      <a href="{verifyUrl}" style="background:#1F4E79;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;">
        Xác thực email
      </a>
      <p>Link có hiệu lực trong <strong>24 giờ</strong>.</p>
    """;
    await SendAsync(toEmail, displayName, "Xác thực email — VocabToeic", body, cancellationToken);
  }

  public async Task SendPasswordResetEmailAsync(
      string toEmail, string displayName, string token,
      CancellationToken cancellationToken = default)
  {
    var resetUrl = $"{_frontendUrl}/reset-password?token={token}";
    var body = $"""
      <h2>Xin chào {displayName}!</h2>
      <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu.</p>
      <a href="{resetUrl}" style="background:#1F4E79;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;">
        Đặt lại mật khẩu
      </a>
      <p>Link có hiệu lực trong <strong>1 giờ</strong>.</p>
    """;
    await SendAsync(toEmail, displayName, "Đặt lại mật khẩu — VocabToeic", body, cancellationToken);
  }

  public async Task SendSetPasswordEmailAsync(
      string toEmail, string displayName, string token,
      CancellationToken cancellationToken = default)
  {
    var setPasswordUrl = $"{_frontendUrl}/set-password?token={token}";
    var body = $"""
      <h2>Xin chào {displayName}!</h2>
      <p>Bạn có thể đặt thêm mật khẩu để đăng nhập bằng email.</p>
      <a href="{setPasswordUrl}" style="background:#1F4E79;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;">
        Đặt mật khẩu
      </a>
      <p>Link có hiệu lực trong <strong>1 giờ</strong>.</p>
    """;
    await SendAsync(toEmail, displayName, "Đặt mật khẩu — VocabToeic", body, cancellationToken);
  }

  private async Task SendAsync(
      string toEmail, string toName, string subject, string htmlBody,
      CancellationToken cancellationToken)
  {
    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mailersend.com/v1/email");
    request.Headers.Add("Authorization", $"Bearer {_apiKey}");
    request.Content = JsonContent.Create(new
    {
      from = new { email = _from, name = _displayName },
      to = new[] { new { email = toEmail, name = toName } },
      subject,
      html = htmlBody
    });

    var response = await _httpClient.SendAsync(request, cancellationToken);
    var body = await response.Content.ReadAsStringAsync(cancellationToken);
    if (!response.IsSuccessStatusCode)
      throw new HttpRequestException($"Mailersend {response.StatusCode}: {body}");
  }
}
