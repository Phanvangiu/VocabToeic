using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Common.Interfaces.Repositories;

public interface IWordRepository : IGenericRepository<Word>
{
  Task<(IEnumerable<Word> Items, int TotalCount)> GetPagedAsync(
      string? topic,
      int? level,
      string? search,
      int page,
      int pageSize,
      CancellationToken ct = default);

  Task<Word?> GetWithDefinitionsAsync(Guid id, CancellationToken ct = default);
  Task<IEnumerable<string>> GetDistinctTopicsAsync(CancellationToken ct = default);
  Task<bool> ExistsByTermAsync(string term, CancellationToken ct = default);
}