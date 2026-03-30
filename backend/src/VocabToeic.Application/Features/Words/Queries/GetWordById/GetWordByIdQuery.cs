using MediatR;
using VocabToeic.Application.Features.Words.DTOs;

namespace VocabToeic.Application.Features.Words.Queries.GetWordById;

public record GetWordByIdQuery(Guid Id) : IRequest<WordDetailResponse>;