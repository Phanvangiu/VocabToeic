using VocabToeic.Domain.Enums;

namespace VocabToeic.Domain.Entities;

public class UserWordProgress : BaseEntity
{
  public Guid UserId { get; set; }
  public Guid WordId { get; set; }

  public WordStatus Status { get; set; } = WordStatus.New;

  // SRS Fields
  public double EaseFactor { get; set; } = 2.5;
  public int IntervalDays { get; set; } = 1;
  public DateTime? NextReviewAt { get; set; }

  // Thống kê
  public int CorrectCount { get; set; } = 0;
  public int IncorrectCount { get; set; } = 0;
  public DateTime? LastSeenAt { get; set; }

  // Navigation
  public User User { get; set; } = null!;
  public Word Word { get; set; } = null!;
}