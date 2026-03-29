using MediatR;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Users.DTOs;

namespace VocabToeic.Application.Features.Users.Commands.UpdateTarget;

public class TargetCommandHandler : IRequestHandler<TargetCommand, TargetResponse>
{
  private readonly IUnitOfWork _uow;
  private readonly ICurrentUserService _currentUserService;

  public TargetCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
  {
    _uow = uow;
    _currentUserService = currentUserService;
  }

  public async Task<TargetResponse> Handle(TargetCommand request, CancellationToken cancellationToken)
  {
    var userId = _currentUserService.UserId;

    await _uow.Users.UpdateTargetAsync(userId, request.TargetScore, request.WordsPerDay, cancellationToken);

    await _uow.SaveChangesAsync(cancellationToken);

    return new TargetResponse
    {
      TargetScore = request.TargetScore,
      WordsPerDay = request.WordsPerDay
    };
  }
}