using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entity.
/// Extends IGenericRepository with user-specific queries.
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
  Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

  Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);
}