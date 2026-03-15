using VocabToeic.Domain.Entities;
using VocabToeic.Domain.Enums;

namespace VocabToeic.Application.Common.Interfaces.Repositories;

public interface IExternalLoginRepository : IGenericRepository<ExternalLogin>
{
  Task<ExternalLogin?> GetByProviderAsync(
      AuthProvider provider,
      string providerKey,
      CancellationToken cancellationToken = default);
  Task<IEnumerable<ExternalLogin>> GetByUserIdAsync(
      Guid userId,
      CancellationToken cancellationToken = default);
}