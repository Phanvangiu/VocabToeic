using MediatR;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Auth.DTOs;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Features.Auth.Commands.Register;

/// <summary>
/// Handles user registration.
/// Handler only knows interfaces — does not know BCrypt, EF Core, or any implementation detail.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, TokenResponse>
{
  private readonly IUnitOfWork _uow;
  private readonly IPasswordService _passwordService;

  public RegisterCommandHandler(
      IUnitOfWork uow,
      IPasswordService passwordService)
  {
    _uow = uow;
    _passwordService = passwordService;
  }

  public async Task<TokenResponse> Handle(
      RegisterCommand request,
      CancellationToken cancellationToken)
  {
    // Check if email already exists
    var emailExists = await _uow.Users.IsEmailTakenAsync(
        request.Email, cancellationToken);

    if (emailExists)
      throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Email", ["Email is already taken."] }
            });

    // Create new user
    var user = new User
    {
      Email = request.Email.ToLower().Trim(),
      PasswordHash = _passwordService.HashPassword(request.Password),
      DisplayName = request.DisplayName.Trim(),
      TargetScore = request.TargetScore,
      Streak = 0,
      IsActive = true
    };

    await _uow.Users.AddAsync(user, cancellationToken);
    await _uow.SaveChangesAsync(cancellationToken);

    // Return basic user info — access token issued after email verification (TODO)
    return new TokenResponse
    {
      AccessToken = string.Empty,
      ExpiresIn = 0,
      User = new UserInfo
      {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        TargetScore = user.TargetScore,
        Streak = user.Streak
      }
    };
  }
}