using VocabToeic.Domain.Enums;

namespace VocabToeic.Domain.Entities;

public class ExternalLogin : BaseEntity
{
  public Guid UserId { get; set; }
  public AuthProvider Provider { get; set; }
  public string ProviderKey { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? AvatarUrl { get; set; }
  public User User { get; set; } = null!;
}