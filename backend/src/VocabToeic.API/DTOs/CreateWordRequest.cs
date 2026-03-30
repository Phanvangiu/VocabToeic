namespace VocabToeic.API.DTOs;

public record CreateDefinitionRequest(
    string? PartOfSpeech,
    string DefinitionEn,
    string DefinitionVi,
    string? ExampleEn,
    string? ExampleVi,
    string? Tips,
    int SortOrder = 0
);

public record CreateWordRequest(
    string Term,
    string? Phonetic,
    string? AudioUrl,
    string Topic,
    int Level,
    IEnumerable<CreateDefinitionRequest> Definitions
);