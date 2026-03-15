using MediatR;
using VocabToeic.Application.Features.Auth.DTOs;

namespace VocabToeic.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisterResponse>
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string ConfirmPassword { get; set; } = string.Empty;
}