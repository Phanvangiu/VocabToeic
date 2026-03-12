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

  private IUserRepository? _users;
  private IRefreshTokenRepository? _refreshTokens;

  public UnitOfWork(AppDbContext context)
  {
    _context = context;
  }

  public IUserRepository Users
      => _users ??= new UserRepository(_context);

  public IRefreshTokenRepository RefreshTokens
      => _refreshTokens ??= new RefreshTokenRepository(_context);

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
      => await _context.SaveChangesAsync(cancellationToken);

  public void Dispose()
      => _context.Dispose();
}