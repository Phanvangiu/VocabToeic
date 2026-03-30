using MediatR;

namespace VocabToeic.Application.Features.Words.Queries.GetTopics;

public record GetTopicsQuery : IRequest<IEnumerable<string>>;