using StackExchange.Redis;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.Infrastructure.Services;

/// <summary>
/// Redis implementation of IRedisService using StackExchange.Redis.
/// Used for JWT blacklist and caching operations.
/// </summary>
public class RedisService : IRedisService
{
  private readonly IDatabase _db;

  public RedisService(IConnectionMultiplexer redis)
  {
    _db = redis.GetDatabase();
  }

  public async Task SetAsync(string key, string value, int expirySeconds)
      => await _db.StringSetAsync(key, value, TimeSpan.FromSeconds(expirySeconds));

  public async Task<bool> ExistsAsync(string key)
      => await _db.KeyExistsAsync(key);

  public async Task DeleteAsync(string key)
      => await _db.KeyDeleteAsync(key);
}