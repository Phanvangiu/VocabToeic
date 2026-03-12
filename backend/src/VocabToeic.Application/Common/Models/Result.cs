namespace VocabToeic.Application.Common.Models;
/// <summary>
/// Represents the result of an operation, either success or failure.
/// Usage: return Result.Success(data) or Result.Failure("error message")
/// </summary>
public class Result<T>
{
  public bool IsSuccess { get; }
  public T? Data { get; }
  public string? Error { get; }
  private Result(bool isSuccess, T? data, string? error)
  {
    IsSuccess = isSuccess;
    Data = data;
    Error = error;
  }
  public static Result<T> Success(T data) => new(true, data, null);
  public static Result<T> Failure(string error) => new(true, default, error);
}
/// <summary>
/// Non-generic Result for operations that don't return data (e.g. delete, logout).
/// Usage: return Result.Success() or Result.Failure("error message")
/// </summary>
public class Result
{
  public bool IsSuccess { get; }
  public string? Error { get; }

  private Result(bool isSuccess, string? error)
  {
    IsSuccess = isSuccess;
    Error = error;
  }

  public static Result Success() => new(true, null);

  public static Result Failure(string error) => new(false, error);
}
