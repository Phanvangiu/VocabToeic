namespace VocabToeic.Domain.Entities;

public class WordDefinition : BaseEntity
{
  public Guid WordId { get; set; }
  public string DefinitionEn { get; set; } = string.Empty;
  public string DefinitionVi { get; set; } = string.Empty;
  public string? PartOfSpeech { get; set; }
  public string? ExampleEn { get; set; }
  public string? ExampleVi { get; set; }
  public string? Tips { get; set; }
  public int SortOrder { get; set; } = 0;

  // Navigation
  public Word Word { get; set; } = null!;
}