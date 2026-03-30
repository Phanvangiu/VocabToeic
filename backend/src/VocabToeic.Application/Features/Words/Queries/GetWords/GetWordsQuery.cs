using MediatR;
using VocabToeic.Application.Common.Models;
using VocabToeic.Application.Features.Words.DTOs;

namespace VocabToeic.Application.Features.Words.Queries.GetWords;

public record GetWordsQuery(
    string? Topic,
    int? Level,
    string? Search,
    int Page = 1,
    int PageSize = 20
) : IRequest<PageResult<WordResponse>>;