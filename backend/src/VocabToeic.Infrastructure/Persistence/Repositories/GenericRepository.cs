using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VocabToeic.Application.Common.Interfaces.Repositories;

namespace VocabToeic.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic repository implementation using EF Core.
/// Provides common CRUD operations for all entities.
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
  protected readonly AppDbContext _context;
  protected readonly DbSet<T> _dbSet;

  public GenericRepository(AppDbContext context)
  {
    _context = context;
    _dbSet = context.Set<T>();
  }

  public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
      => await _dbSet.FindAsync([id], cancellationToken);

  public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
      => await _dbSet.ToListAsync(cancellationToken);

  public async Task<T?> FirstOrDefaultAsync(
      Expression<Func<T, bool>> predicate,
      CancellationToken cancellationToken = default)
      => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

  public async Task<bool> ExistsAsync(
      Expression<Func<T, bool>> predicate,
      CancellationToken cancellationToken = default)
      => await _dbSet.AnyAsync(predicate, cancellationToken);

  public async Task<int> CountAsync(
      Expression<Func<T, bool>> predicate,
      CancellationToken cancellationToken = default)
      => await _dbSet.CountAsync(predicate, cancellationToken);

  public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
      => await _dbSet.AddAsync(entity, cancellationToken);

  public void Update(T entity)
      => _dbSet.Update(entity);

  public void Remove(T entity)
      => _dbSet.Remove(entity);
}