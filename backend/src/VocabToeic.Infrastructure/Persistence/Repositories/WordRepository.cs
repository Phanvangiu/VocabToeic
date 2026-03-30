using Microsoft.EntityFrameworkCore;
using VocabToeic.Application.Common.Interfaces.Repositories;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Repositories;

public class WordRepository : GenericRepository<Word>, IWordRepository
{
  public WordRepository(AppDbContext context) : base(context) { }

  public async Task<(IEnumerable<Word> Items, int TotalCount)> GetPagedAsync(
      string? topic,
      int? level,
      string? search,
      int page,
      int pageSize,
      CancellationToken ct = default)
  {
    var query = _dbSet.AsNoTracking();

    if (!string.IsNullOrWhiteSpace(topic))
      query = query.Where(w => w.Topic == topic);

    if (level.HasValue)
      query = query.Where(w => w.Level == level.Value);

    if (!string.IsNullOrWhiteSpace(search))
      query = query.Where(w => w.Term.Contains(search));

    var total = await query.CountAsync(ct);

    var items = await query
        .OrderBy(w => w.Term)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(w => new Word
        {
          Id = w.Id,
          Term = w.Term,
          Phonetic = w.Phonetic,
          AudioUrl = w.AudioUrl,
          Topic = w.Topic,
          Level = w.Level,
          Definitions = w.Definitions
        })
        .ToListAsync(ct);

    return (items, total);
  }

  public async Task<Word?> GetWithDefinitionsAsync(Guid id, CancellationToken ct = default)
      => await _dbSet
          .AsNoTracking()
          .Include(w => w.Definitions.OrderBy(d => d.SortOrder))
          .FirstOrDefaultAsync(w => w.Id == id, ct);

  public async Task<IEnumerable<string>> GetDistinctTopicsAsync(CancellationToken ct = default)
      => await _dbSet
          .AsNoTracking()
          .Select(w => w.Topic)
          .Distinct()
          .OrderBy(t => t)
          .ToListAsync(ct);

  public async Task<bool> ExistsByTermAsync(string term, CancellationToken ct = default)
      => await _dbSet.AnyAsync(w => w.Term == term, ct);
}