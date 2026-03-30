using MediatR;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Words.DTOs;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Features.Words.Queries.GetWordById;

public class GetWordByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetWordByIdQuery, WordDetailResponse>
{
  public async Task<WordDetailResponse> Handle(
      GetWordByIdQuery request,
      CancellationToken cancellationToken)
  {
    var word = await uow.Words.GetWithDefinitionsAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException(nameof(Word), request.Id);

    return new WordDetailResponse(
        word.Id,
        word.Term,
        word.Phonetic,
        word.AudioUrl,
        word.Topic,
        word.Level,
        word.Definitions.Select(d => new WordDefinitionDto(
            d.Id,
            d.PartOfSpeech,
            d.DefinitionEn,
            d.DefinitionVi,
            d.ExampleEn,
            d.ExampleVi,
            d.Tips,
            d.SortOrder)));
  }
}