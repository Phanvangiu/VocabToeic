namespace VocabToeic.Application.Features.Words.DTOs;

public record WordDetailResponse(
    Guid Id,
    string Term,
    string? Phonetic,
    string? AudioUrl,
    string Topic,
    int Level,
    IEnumerable<WordDefinitionDto> Definitions
);