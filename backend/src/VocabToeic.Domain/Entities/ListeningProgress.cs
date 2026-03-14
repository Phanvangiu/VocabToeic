namespace VocabToeic.Domain.Entities;

public class ListeningProgress : BaseEntity
{
  public Guid UserId { get; set; }
  public Guid ExerciseId { get; set; }

  public int ListenedCount { get; set; } = 0;

  public DateTime? LastListenedAt { get; set; }

  public bool IsCompleted { get; set; } = false;

  public User User { get; set; } = null!;
  public Exercise Exercise { get; set; } = null!;
}