namespace VocabToeic.Infrastructure.Services;

/// <summary>
/// Provides password hashing and verification using BCrypt.
/// </summary>
public interface IPasswordService
{
  /// <summary>
  /// Hashes a plain text password using BCrypt with salt rounds = 12.
  /// </summary>
  /// <param name="password">Plain text password.</param>
  /// <returns>BCrypt hashed password string.</returns>
  string HashPassword(string password);

  /// <summary>
  /// Verifies a plain text password against a BCrypt hashed password.
  /// </summary>
  /// <param name="password">Plain text password to verify.</param>
  /// <param name="hashedPassword">BCrypt hashed password from DB.</param>
  /// <returns>True if password matches, false otherwise.</returns>
  bool VerifyPassword(string password, string hashedPassword);
}