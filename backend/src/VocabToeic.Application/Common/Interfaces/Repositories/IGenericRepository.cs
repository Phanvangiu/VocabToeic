using System.Linq.Expressions;

namespace VocabToeic.Application.Common.Interfaces.Repositories;

/// <summary>
/// Generic repository interface providing common CRUD operations for all entities.
/// Specific repositories extend this interface with domain-specific queries.
/// </summary>
public interface IGenericRepository<T> where T : class
{
  Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
  Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

  Task AddAsync(T entity, CancellationToken cancellationToken = default);
  void Update(T entity);
  void Remove(T entity);
}