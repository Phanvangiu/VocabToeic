using MediatR;
using VocabToeic.Application.Common.Interfaces;

namespace VocabToeic.Application.Features.Words.Queries.GetTopics;

public class GetTopicsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetTopicsQuery, IEnumerable<string>>
{
  public async Task<IEnumerable<string>> Handle(
      GetTopicsQuery request,
      CancellationToken cancellationToken)
      => await uow.Words.GetDistinctTopicsAsync(cancellationToken);
}