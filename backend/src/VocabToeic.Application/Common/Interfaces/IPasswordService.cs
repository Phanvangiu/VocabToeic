namespace VocabToeic.Application.Common.Interfaces;

/// <summary>
/// Provides password hashing and verification operations.
/// </summary>
public interface IPasswordService
{
  string HashPassword(string password);
  bool VerifyPassword(string password, string hashedPassword);
}