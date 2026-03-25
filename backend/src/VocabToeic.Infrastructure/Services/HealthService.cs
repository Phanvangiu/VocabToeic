using Microsoft.EntityFrameworkCore;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Infrastructure.Persistence;

namespace VocabToeic.Infrastructure.Services
{
  // VocabToeic.Infrastructure/Services/HealthService.cs
  public class HealthService : IHealthService
  {
    private readonly AppDbContext _db;
    private readonly IRedisService _redis;

    public HealthService(AppDbContext db, IRedisService redis)
    {
      _db = db;
      _redis = redis;
    }

    public async Task<bool> PingDatabaseAsync(CancellationToken cancellationToken = default)
    {
      await _db.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
      await _redis.PingAsync();
      return true;
    }
  }

}