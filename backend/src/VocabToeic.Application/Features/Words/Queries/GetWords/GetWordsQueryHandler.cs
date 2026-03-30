using MediatR;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Common.Models;
using VocabToeic.Application.Features.Words.DTOs;

namespace VocabToeic.Application.Features.Words.Queries.GetWords;

public class GetWordsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetWordsQuery, PageResult<WordResponse>>
{
  public async Task<PageResult<WordResponse>> Handle(
      GetWordsQuery request,
      CancellationToken cancellationToken)
  {
    var (items, total) = await uow.Words.GetPagedAsync(
        request.Topic,
        request.Level,
        request.Search,
        request.Page,
        request.PageSize,
        cancellationToken);

    var dtos = items.Select(w => new WordResponse(
        w.Id,
        w.Term,
        w.Phonetic,
        w.AudioUrl,
        w.Topic,
        w.Level,
        w.Definitions.Count));

    return new PageResult<WordResponse>(dtos.ToList(), total, request.Page, request.PageSize);
  }
}