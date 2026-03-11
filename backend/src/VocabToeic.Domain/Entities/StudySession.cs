namespace VocabToeic.Domain.Entities;

public class StudySession : BaseEntity
{
  public Guid UserId { get; set; }
  public DateTime StartedAt { get; set; } = DateTime.UtcNow;
  public DateTime? EndedAt { get; set; }
  public int WordsReviewed { get; set; } = 0;
  public int ExercisesDone { get; set; } = 0;

  // Navigation
  public User User { get; set; } = null!;
}