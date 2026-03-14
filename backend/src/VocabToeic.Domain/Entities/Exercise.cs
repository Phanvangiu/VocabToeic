using VocabToeic.Domain.Enums;

namespace VocabToeic.Domain.Entities;

/// <summary>
/// Bài tập TOEIC.
/// Part 1-4 (Listening): dùng ListeningContent — có audio_url, transcript.
/// Part 5-7 (Reading):   dùng ReadingContent  — có passage, questions.
/// Chỉ một trong hai có giá trị, cái còn lại null.
/// </summary>
public class Exercise : BaseEntity
{
  public ExercisePart Part { get; set; }
  public string Topic { get; set; } = string.Empty;
  public int Difficulty { get; set; } = 1;
  public bool IsAiGenerated { get; set; } = true;

  /// <summary>Dùng cho Part 5-7. Null nếu là bài Listening.</summary>
  public ReadingContent? ReadingContent { get; set; }

  /// <summary>Dùng cho Part 1-4. Null nếu là bài Reading.</summary>
  public ListeningContent? ListeningContent { get; set; }

  public ICollection<ExerciseResult> Results { get; set; } = [];
  public ICollection<ListeningProgress> ListeningProgresses { get; set; } = [];

  public bool IsListening =>
      Part is ExercisePart.Part1 or ExercisePart.Part2
             or ExercisePart.Part3 or ExercisePart.Part4;

  public bool IsReading =>
      Part is ExercisePart.Part5 or ExercisePart.Part6 or ExercisePart.Part7;
}

// ── Reading Content (Part 5-7) ────────────────────────────────────────────────

public class ReadingContent
{
  /// <summary>"single" | "double" | "triple". Null với Part 5.</summary>
  public string? PassageType { get; set; }
  public List<Passage>? Passages { get; set; }
  public List<ReadingQuestion> Questions { get; set; } = [];
}

public class Passage
{
  public string? Title { get; set; }
  public string Body { get; set; } = string.Empty;
}

public class ReadingQuestion
{
  public int Order { get; set; }
  /// <summary>Câu có chỗ trống — Part 5.</summary>
  public string? Sentence { get; set; }
  /// <summary>Câu hỏi đọc hiểu — Part 6/7.</summary>
  public string? Question { get; set; }
  public List<string> Options { get; set; } = [];
  public string CorrectAnswer { get; set; } = string.Empty;
  public string ExplanationVi { get; set; } = string.Empty;
}


public class ListeningContent
{
  /// <summary>URL file MP3. Null nếu chưa upload audio.</summary>
  public string? AudioUrl { get; set; }
  /// <summary>Lời thoại đầy đủ. Hiển thị sau khi nộp bài.</summary>
  public string? Transcript { get; set; }
  /// <summary>URL ảnh — chỉ Part 1 (Photographs).</summary>
  public string? ImageUrl { get; set; }
  public List<ListeningQuestion> Questions { get; set; } = [];
}

public class ListeningQuestion
{
  public int Order { get; set; }
  /// <summary>Câu hỏi text — Part 3/4. Null với Part 1/2 (câu hỏi trong audio).</summary>
  public string? Question { get; set; }
  /// <summary>Part 1/2: 3 options. Part 3/4: 4 options.</summary>
  public List<string> Options { get; set; } = [];
  public string CorrectAnswer { get; set; } = string.Empty;
  public string ExplanationVi { get; set; } = string.Empty;
}