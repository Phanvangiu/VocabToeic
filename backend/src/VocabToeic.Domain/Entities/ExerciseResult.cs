namespace VocabToeic.Domain.Entities;

public class ExerciseResult : BaseEntity
{
  public Guid UserId { get; set; }
  public Guid ExerciseId { get; set; }
  public int Score { get; set; }
  public int TotalQuestions { get; set; }
  public int TimeTakenSec { get; set; }
  public Dictionary<string, string> Answers { get; set; } = [];
  public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

  // Navigation
  public User User { get; set; } = null!;
  public Exercise Exercise { get; set; } = null!;
}