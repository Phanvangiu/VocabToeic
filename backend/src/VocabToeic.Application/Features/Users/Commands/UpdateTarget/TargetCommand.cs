using MediatR;
using VocabToeic.Application.Features.Users.DTOs;

namespace VocabToeic.Application.Features.Users.Commands.UpdateTarget;

public class TargetCommand : IRequest<TargetResponse>
{
  public int TargetScore { get; set; }
  public int WordsPerDay { get; set; }
}