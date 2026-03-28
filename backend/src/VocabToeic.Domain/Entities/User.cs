using VocabToeic.Domain.Enums;

namespace VocabToeic.Domain.Entities;

public class User : BaseEntity
{
  public string Email { get; set; } = string.Empty;

  /// <summary>Null nếu user đăng ký qua OAuth.</summary>
  public string? PasswordHash { get; set; }

  public string DisplayName { get; set; } = string.Empty;
  public string? AvatarUrl { get; set; }
  public UserRole Role { get; set; } = UserRole.User;
  public int? TargetScore { get; set; }
  public int Streak { get; set; } = 0;
  public int? WordsPerDay { get; set; }
  public DateTime? LastStudyDate { get; set; }
  public bool IsActive { get; set; } = true;

  // Email Verification  
  public bool EmailVerified { get; set; } = false;
  public string? EmailVerificationToken { get; set; }
  public DateTime? EmailVerificationExpiresAt { get; set; }

  // Password Reset
  public string? PasswordResetToken { get; set; }
  public DateTime? PasswordResetExpiresAt { get; set; }

  public ICollection<UserWordProgress> WordProgresses { get; set; } = [];
  public ICollection<ExerciseResult> ExerciseResults { get; set; } = [];
  public ICollection<StudySession> StudySessions { get; set; } = [];
  public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
  public ICollection<ListeningProgress> ListeningProgresses { get; set; } = [];
  public ICollection<ExternalLogin> ExternalLogins { get; set; } = [];
}