using Microsoft.EntityFrameworkCore;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Infrastructure.Persistence;

namespace VocabToeic.Infrastructure.Services
{
  // VocabToeic.Infrastructure/Services/HealthService.cs
  public class HealthService : IHealthService
  {
    private readonly AppDbContext _db;

    public HealthService(AppDbContext db)
    {
      _db = db;
    }

    public async Task<bool> PingDatabaseAsync(CancellationToken cancellationToken = default)
    {
      await _db.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
      return true;
    }
  }

}