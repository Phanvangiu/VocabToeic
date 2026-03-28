using Microsoft.EntityFrameworkCore;
using VocabToeic.Application.Common.Interfaces.Repositories;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IRefreshTokenRepository.
/// Handles token lookup, revocation, and cleanup operations.
/// </summary>
public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
  public RefreshTokenRepository(AppDbContext context) : base(context) { }

  public async Task<RefreshToken?> GetByHashedTokenAsync(
      string hashedToken,
      CancellationToken cancellationToken = default)
      => await _dbSet
          .Include(rt => rt.User)
          .FirstOrDefaultAsync(rt => rt.Token == hashedToken, cancellationToken);

  public async Task RevokeAllUserTokensAsync(
      Guid userId,
      string reason,
      CancellationToken cancellationToken = default)
  {
    var activeTokens = await _dbSet
        .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
        .ToListAsync(cancellationToken);

    foreach (var token in activeTokens)
    {
      token.RevokedAt = DateTime.UtcNow;
      token.RevokedReason = reason;
    }
  }

  public async Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
  {
    await _dbSet
        .Where(rt => rt.ExpiresAt < DateTime.UtcNow || rt.RevokedAt != null)
        .ExecuteDeleteAsync(cancellationToken);
  }
}