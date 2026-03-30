using Microsoft.EntityFrameworkCore.Storage;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Common.Interfaces.Repositories;
using VocabToeic.Infrastructure.Persistence.Repositories;

namespace VocabToeic.Infrastructure.Persistence;

/// <summary>
/// Implements IUnitOfWork — aggregates all repositories and manages DB transactions.
/// All repositories share the same DbContext instance to ensure atomic operations.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
  private readonly AppDbContext _context;
  private IDbContextTransaction? _currentTransaction;

  private IUserRepository? _users;
  private IRefreshTokenRepository? _refreshTokens;
  private IExternalLoginRepository? _externalLogins;
  private IWordRepository? _words;

  public UnitOfWork(AppDbContext context)
  {
    _context = context;
  }

  public IUserRepository Users
      => _users ??= new UserRepository(_context);

  public IRefreshTokenRepository RefreshTokens
      => _refreshTokens ??= new RefreshTokenRepository(_context);

  public IExternalLoginRepository ExternalLogins
      => _externalLogins ??= new ExternalLoginRepository(_context);

  public IWordRepository Words
      => _words ??= new WordRepository(_context);

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
      => await _context.SaveChangesAsync(cancellationToken);

  public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
  {
    _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
  }

  public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_currentTransaction is null)
      throw new InvalidOperationException("No active transaction to commit.");

    await _currentTransaction.CommitAsync(cancellationToken);
    await _currentTransaction.DisposeAsync();
    _currentTransaction = null;
  }

  public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_currentTransaction is null) return;

    await _currentTransaction.RollbackAsync(cancellationToken);
    await _currentTransaction.DisposeAsync();
    _currentTransaction = null;
  }

  public void Dispose()
  {
    _currentTransaction?.Dispose();
    _context.Dispose();
  }
}