using MediatR;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Words.DTOs;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Features.Words.Commands.CreateWord;

public class CreateWordCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateWordCommand, WordDetailResponse>
{
  public async Task<WordDetailResponse> Handle(
      CreateWordCommand request,
      CancellationToken cancellationToken)
  {
    var term = request.Term.Trim();

    if (await uow.Words.ExistsByTermAsync(term, cancellationToken))
      throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(request.Term), [$"Term '{term}' already exists."] }
            });

    await uow.BeginTransactionAsync(cancellationToken);
    try
    {
      var word = new Word
      {
        Term = term,
        Phonetic = request.Phonetic?.Trim(),
        AudioUrl = request.AudioUrl?.Trim(),
        Topic = request.Topic,
        Level = request.Level,
        Definitions = request.Definitions.Select(d => new WordDefinition
        {
          PartOfSpeech = d.PartOfSpeech?.Trim(),
          DefinitionEn = d.DefinitionEn.Trim(),
          DefinitionVi = d.DefinitionVi.Trim(),
          ExampleEn = d.ExampleEn?.Trim(),
          ExampleVi = d.ExampleVi?.Trim(),
          Tips = d.Tips?.Trim(),
          SortOrder = d.SortOrder
        }).ToList()
      };

      await uow.Words.AddAsync(word, cancellationToken);
      await uow.SaveChangesAsync(cancellationToken);
      await uow.CommitTransactionAsync(cancellationToken);

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
    catch
    {
      await uow.RollbackTransactionAsync(cancellationToken);
      throw;
    }
  }
}