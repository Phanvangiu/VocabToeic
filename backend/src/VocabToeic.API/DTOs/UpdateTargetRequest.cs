namespace VocabToeic.API.DTOs;

public class UpdateTargetRequest
{
  public int TargetScore { get; set; }
  public int WordsPerDay { get; set; }
}