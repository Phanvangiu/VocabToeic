namespace VocabToeic.Domain.Entities;

public class RefreshToken : BaseEntity
{
  public Guid UserId { get; set; }
  public string Token { get; set; } = string.Empty;
  public string? DeviceInfo { get; set; }
  public string? IpAddress { get; set; }
  public DateTime ExpiresAt { get; set; }
  public DateTime? RevokedAt { get; set; }
  public string? RevokedReason { get; set; }

  public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

  public User User { get; set; } = null!;
}