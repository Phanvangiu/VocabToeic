namespace VocabToeic.Application.Common.Interfaces
{
  public interface IHealthService
  {
    Task<bool> PingDatabaseAsync(CancellationToken cancellationToken = default);
  }
}