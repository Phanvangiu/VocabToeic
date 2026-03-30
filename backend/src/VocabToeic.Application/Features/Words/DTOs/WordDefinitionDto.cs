namespace VocabToeic.Application.Features.Words.DTOs;

public record WordDefinitionDto(
    Guid Id,
    string? PartOfSpeech,
    string DefinitionEn,
    string DefinitionVi,
    string? ExampleEn,
    string? ExampleVi,
    string? Tips,
    int SortOrder
);