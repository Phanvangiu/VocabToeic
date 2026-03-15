using VocabToeic.Application.Common.Interfaces.Repositories;

namespace VocabToeic.Application.Common.Interfaces;

/// <summary>
/// Aggregates all repositories and manages database transactions.
/// Ensures multiple operations are committed atomically in a single transaction.
/// Usage: await _uow.SaveChangesAsync() to commit all pending changes.
/// </summary>
public interface IUnitOfWork : IDisposable
{
  IUserRepository Users { get; }
  IRefreshTokenRepository RefreshTokens { get; }
  IExternalLoginRepository ExternalLogins { get; }

  /// <summary>Commits all pending changes to the database.</summary>
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}