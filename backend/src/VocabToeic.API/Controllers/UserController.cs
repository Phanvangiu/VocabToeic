using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocabToeic.API.DTOs;
using VocabToeic.Application.Features.Users.Commands.UpdateTarget;
using VocabToeic.Application.Features.Users.DTOs;

namespace VocabToeic.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
  /// <summary>Update TOEIC target score and daily vocabulary goal.</summary>
  [HttpPut("me/target")]
  [ProducesResponseType(typeof(TargetResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> UpdateTarget(
      [FromBody] UpdateTargetRequest request,
      CancellationToken cancellationToken)
  {
    var command = new TargetCommand
    {
      TargetScore = request.TargetScore,
      WordsPerDay = request.WordsPerDay
    };

    var result = await Mediator.Send(command, cancellationToken);
    return Ok(new { message = "Update target successfully.", data = result });
  }
}