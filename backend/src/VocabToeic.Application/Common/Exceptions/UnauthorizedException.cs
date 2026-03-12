namespace VocabToeic.Application.Common.Exceptions;
/// <summary>
/// Exception thrown when a user is not authenticated or does not have permission.
/// Usage: throw new UnauthorizedException("reason message")
/// </summary>
public class UnauthorizedException : Exception
{
  public UnauthorizedException(string message) : base(message) { }
}