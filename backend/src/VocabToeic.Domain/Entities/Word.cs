namespace VocabToeic.Domain.Entities;

public class Word : BaseEntity
{
  public string Term { get; set; } = string.Empty;
  public string? Phonetic { get; set; }
  public string? AudioUrl { get; set; }
  public string? PartOfSpeech { get; set; }
  public string Topic { get; set; } = string.Empty;
  public int Level { get; set; } = 1;

  // Navigation
  public ICollection<WordDefinition> Definitions { get; set; } = [];
  public ICollection<UserWordProgress> UserProgresses { get; set; } = [];
}