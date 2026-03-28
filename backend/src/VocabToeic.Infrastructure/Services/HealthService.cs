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
    private readonly IUnitOfWork _uow;

    public HealthService(AppDbContext db, IRedisService redis, IUnitOfWork uow)
    {
      _db = db;
      _redis = redis;
      _uow = uow;
    }

    public async Task<bool> PingDatabaseAsync(CancellationToken cancellationToken = default)
    {
      await _db.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
      await _redis.PingAsync();
      await _uow.RefreshTokens.DeleteExpiredTokensAsync(cancellationToken);

      return true;
    }
  }

}