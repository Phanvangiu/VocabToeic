using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocabToeic.API.DTOs;
using VocabToeic.Application.Features.Words.Commands.CreateWord;
using VocabToeic.Application.Features.Words.DTOs;

namespace VocabToeic.API.Controllers;

public class WordsController : BaseApiController
{
  /// <summary>Create a new word with definitions. Admin only.</summary>
  [Authorize(Roles = "Admin")]
  [HttpPost]
  [ProducesResponseType(typeof(WordDetailResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<IActionResult> CreateWord(
   [FromBody] CreateWordRequest request,
   CancellationToken cancellationToken)
  {
    var command = new CreateWordCommand(
        request.Term,
        request.Phonetic,
        request.AudioUrl,
        request.Topic,
        request.Level,
        request.Definitions.Select(d => new Application.Features.Words.Commands.CreateWord.CreateDefinitionRequest(
            d.PartOfSpeech,
            d.DefinitionEn,
            d.DefinitionVi,
            d.ExampleEn,
            d.ExampleVi,
            d.Tips,
            d.SortOrder)));

    var result = await Mediator.Send(command, cancellationToken);
    return Created($"/api/words/{result.Id}", result);
  }
}