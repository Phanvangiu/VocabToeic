namespace VocabToeic.Application.Common.Interfaces;

/// <summary>
/// Provides Redis cache operations.
/// Used for JWT blacklist and other caching needs.
/// </summary>
public interface IRedisService
{
  /// <summary>Sets a key with expiry in seconds.</summary>
  Task SetAsync(string key, string value, int expirySeconds);

  /// <summary>Checks if a key exists.</summary>
  Task<bool> ExistsAsync(string key);

  /// <summary>Deletes a key.</summary>
  Task DeleteAsync(string key);
}