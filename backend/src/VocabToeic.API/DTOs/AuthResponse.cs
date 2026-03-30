namespace VocabToeic.API.DTOs
{
  public class AuthResponse
  {
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public UserInfo User { get; set; } = null!;
  }

}