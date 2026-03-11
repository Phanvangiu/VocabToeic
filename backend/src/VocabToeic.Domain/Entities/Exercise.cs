using VocabToeic.Domain.Enums;

namespace VocabToeic.Domain.Entities;

public class Exercise : BaseEntity
{
  public ExercisePart Part { get; set; }
  public string Topic { get; set; } = string.Empty;
  public int Difficulty { get; set; } = 1;
  public bool IsAiGenerated { get; set; } = true;
  public ExerciseContent Content { get; set; } = null!;

  // Navigation
  public ICollection<ExerciseResult> Results { get; set; } = [];
}

public class ExerciseContent
{
  public string? PassageType { get; set; }
  public List<Passage>? Passages { get; set; }
  public List<QuestionItem> Questions { get; set; } = [];
}

public class Passage
{
  public string? Title { get; set; }
  public string Body { get; set; } = string.Empty;
}

public class QuestionItem
{
  public int Id { get; set; }
  public string? Sentence { get; set; }
  public string? Question { get; set; }
  public List<string> Options { get; set; } = [];
  public string Answer { get; set; } = string.Empty;
  public string ExplanationVi { get; set; } = string.Empty;
}