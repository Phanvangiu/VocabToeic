using MediatR;
using VocabToeic.Application.Features.Words.DTOs;

namespace VocabToeic.Application.Features.Words.Commands.CreateWord;

public record CreateDefinitionRequest(
    string? PartOfSpeech,
    string DefinitionEn,
    string DefinitionVi,
    string? ExampleEn,
    string? ExampleVi,
    string? Tips,
    int SortOrder = 0
);

public record CreateWordCommand(
    string Term,
    string? Phonetic,
    string? AudioUrl,
    string Topic,
    int Level,
    IEnumerable<CreateDefinitionRequest> Definitions
) : IRequest<WordDetailResponse>;