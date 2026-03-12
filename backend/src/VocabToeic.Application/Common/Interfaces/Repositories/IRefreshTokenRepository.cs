using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for RefreshToken entity.
/// Handles token lookup, revocation, and cleanup operations.
/// </summary>
public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
  Task<RefreshToken?> GetByHashedTokenAsync(string hashedToken, CancellationToken cancellationToken = default);

  Task RevokeAllUserTokensAsync(Guid userId, string reason, CancellationToken cancellationToken = default);

  Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default);
}