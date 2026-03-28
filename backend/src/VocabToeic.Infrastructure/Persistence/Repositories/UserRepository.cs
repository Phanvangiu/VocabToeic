using Microsoft.EntityFrameworkCore;
using VocabToeic.Application.Common.Interfaces.Repositories;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IUserRepository.
/// Inherits common CRUD from GenericRepository.
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
  public UserRepository(AppDbContext context) : base(context) { }

  public async Task<User?> GetByEmailAsync(
      string email,
      CancellationToken cancellationToken = default)
      => await _dbSet
          .FirstOrDefaultAsync(u => u.Email == email.ToLower(), cancellationToken);

  public async Task<bool> IsEmailTakenAsync(
      string email,
      CancellationToken cancellationToken = default)
      => await _dbSet
          .AnyAsync(u => u.Email == email.ToLower(), cancellationToken);

  public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
  {
    _context.Users.Remove(user);
    return Task.CompletedTask;
  }
}