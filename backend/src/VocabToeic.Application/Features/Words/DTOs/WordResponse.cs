namespace VocabToeic.Application.Features.Words.DTOs;

public record WordResponse(
    Guid Id,
    string Term,
    string? Phonetic,
    string? AudioUrl,
    string Topic,
    int Level,
    int DefinitionCount
);