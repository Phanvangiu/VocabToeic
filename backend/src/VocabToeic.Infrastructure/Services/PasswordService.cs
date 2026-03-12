
namespace VocabToeic.Infrastructure.Services;

using VocabToeic.Application.Common.Interfaces;

/// <summary>
/// Implements password hashing and verification using BCrypt algorithm.
/// Salt rounds = 12 — balances security and performance.
/// Higher rounds = more secure but slower (12 is industry standard).
/// </summary>
public class PasswordService : IPasswordService
{
  private const int SaltRounds = 12;
  public string HashPassword(string password)
  {
    return BCrypt.Net.BCrypt.HashPassword(password, SaltRounds);
  }
  public bool VerifyPassword(string password, string hashedPassword)
  {
    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
  }
}