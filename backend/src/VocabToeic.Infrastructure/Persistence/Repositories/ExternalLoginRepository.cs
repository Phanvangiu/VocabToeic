using Microsoft.EntityFrameworkCore;
using VocabToeic.Application.Common.Interfaces.Repositories;
using VocabToeic.Domain.Entities;
using VocabToeic.Domain.Enums;

namespace VocabToeic.Infrastructure.Persistence.Repositories;

public class ExternalLoginRepository : GenericRepository<ExternalLogin>, IExternalLoginRepository
{
  public ExternalLoginRepository(AppDbContext context) : base(context) { }

  public async Task<ExternalLogin?> GetByProviderAsync(
      AuthProvider provider,
      string providerKey,
      CancellationToken cancellationToken = default)
      => await _context.ExternalLogins
          .Include(x => x.User)
          .FirstOrDefaultAsync(
              x => x.Provider == provider && x.ProviderKey == providerKey,
              cancellationToken);

  public async Task<IEnumerable<ExternalLogin>> GetByUserIdAsync(
      Guid userId,
      CancellationToken cancellationToken = default)
      => await _context.ExternalLogins
          .Where(x => x.UserId == userId)
          .ToListAsync(cancellationToken);
}