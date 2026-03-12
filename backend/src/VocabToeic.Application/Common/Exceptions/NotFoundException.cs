namespace VocabToeic.Application.Common.Exceptions;
/// <summary>
/// Exception thrown when a requested resource is not found in the database.
/// Usage: throw new NotFoundException(nameof(Entity), id)
/// </summary>
public class NotFoundException : Exception
{
  public NotFoundException(string entityName, object key) : base($"{entityName} with key '{key}' was not found.") { }
}
