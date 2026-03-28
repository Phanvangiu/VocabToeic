namespace VocabToeic.API.DTOs
{
  public class UserInfo
  {
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int? TargetScore { get; set; }
    public int Streak { get; set; }
    public string? AvatarUrl { get; set; }
  }
}