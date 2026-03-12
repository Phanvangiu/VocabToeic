namespace VocabToeic.Application.Common.Models;
/// <summary>
/// Represents a paginated result set for list queries.
/// Usage: return new PagedResult&lt;WordDto&gt;(items, totalCount, page, pageSize)
/// </summary>
public class PageResult<T>
{
  public IReadOnlyList<T> Items { get; }
  public int TotalCount { get; }
  public int Page { get; }
  public int PageSize { get; }

  public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
  public bool HasNextPage => Page < TotalPages;
  public bool HaspreviousPage => Page > 1;

  public PageResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
  {
    Items = items;
    TotalCount = totalCount;
    Page = page;
    PageSize = pageSize;
  }
}