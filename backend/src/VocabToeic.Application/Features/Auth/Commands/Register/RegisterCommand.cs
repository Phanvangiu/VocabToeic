using MediatR;
using VocabToeic.Application.Features.Auth.DTOs;

namespace VocabToeic.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisterResponse>
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string FullName { get; set; } = string.Empty;
  public int TargetScore { get; set; }
  public int WordsPerDay { get; set; }

  public string ConfirmPassword { get; set; } = string.Empty;
}