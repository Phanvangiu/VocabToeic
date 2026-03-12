using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VocabToeic.API.Controllers;

/// <summary>
/// Base controller for all API controllers.
/// Provides MediatR instance — eliminates repetitive injection in each controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
  private IMediator? _mediator;

  // Lazy inject via HttpContext — avoids constructor injection in every controller
  protected IMediator Mediator =>
      _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}