using VocabToeic.Application.Common.Interfaces.Repositories;

namespace VocabToeic.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
  IUserRepository Users { get; }
  IRefreshTokenRepository RefreshTokens { get; }
  IExternalLoginRepository ExternalLogins { get; }
  IWordRepository Words { get; }

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

  Task BeginTransactionAsync(CancellationToken cancellationToken = default);
  Task CommitTransactionAsync(CancellationToken cancellationToken = default);
  Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}