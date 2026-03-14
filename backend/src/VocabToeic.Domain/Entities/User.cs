using VocabToeic.Domain.Enums;

namespace VocabToeic.Domain.Entities;

public class User : BaseEntity
{
  public string Email { get; set; } = string.Empty;

  public string? PasswordHash { get; set; }

  public string DisplayName { get; set; } = string.Empty;
  public string? AvatarUrl { get; set; }
  public UserRole Role { get; set; } = UserRole.User;
  public int? TargetScore { get; set; }
  public int Streak { get; set; } = 0;
  public DateTime? LastStudyDate { get; set; }
  public bool IsActive { get; set; } = true;

  public ICollection<UserWordProgress> WordProgresses { get; set; } = [];
  public ICollection<ExerciseResult> ExerciseResults { get; set; } = [];
  public ICollection<StudySession> StudySessions { get; set; } = [];
  public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
  public ICollection<ListeningProgress> ListeningProgresses { get; set; } = [];
  public ICollection<ExternalLogin> ExternalLogins { get; set; } = [];
}
