using System.Net;
using System.Text.Json;
using VocabToeic.Application.Common.Exceptions;

namespace VocabToeic.API.Middlewares;

/// <summary>
/// Global middleware that catches all unhandled exceptions and returns
/// a standardized JSON error response with the appropriate HTTP status code.
/// </summary>
public class GlobalExceptionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<GlobalExceptionMiddleware> _logger;

  public GlobalExceptionMiddleware(
      RequestDelegate next,
      ILogger<GlobalExceptionMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An unhandled exception occurred");
      await HandleExceptionAsync(context, ex);
    }
  }

  private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    context.Response.ContentType = "application/json";

    // Map exception type to HTTP status code and error response
    int statusCode;
    object response;
    switch (exception)
    {
      case ValidationException ex:
        statusCode = (int)HttpStatusCode.BadRequest;
        response = new { errors = ex.Errors };
        break;
      case NotFoundException ex:
        statusCode = (int)HttpStatusCode.NotFound;
        response = new
        {
          errors = new Dictionary<string, string[]>
        {
          {"message", [ex.Message]}
        }
        };
        break;
      case UnauthorizedException ex:
        statusCode = (int)HttpStatusCode.Unauthorized;
        response = new
        {
          errors = new Dictionary<string, string[]> {
                    { "message", [ex.Message] }
                }
        };
        break;
      case ForbiddenException ex:
        statusCode = (int)HttpStatusCode.Forbidden;
        response = new
        {
          errors = new Dictionary<string, string[]> {
                    { "message", [ex.Message] }
                }
        };
        break;
      default:
        statusCode = (int)HttpStatusCode.InternalServerError;
        response = new
        {
          errors = new Dictionary<string, string[]> {
                    { "message", ["An unexpected error occurred"] }
                }
        };
        break;


    }
    context.Response.StatusCode = statusCode;

    var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    await context.Response.WriteAsync(json);
  }
}