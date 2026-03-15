using MediatR;
using VocabToeic.Application.Common.Exceptions;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Application.Features.Auth.DTOs;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
  private readonly IUnitOfWork _uow;
  private readonly IPasswordService _passwordService;
  private readonly IEmailService _emailService;

  public RegisterCommandHandler(
      IUnitOfWork uow,
      IPasswordService passwordService,
      IEmailService emailService)
  {
    _uow = uow;
    _passwordService = passwordService;
    _emailService = emailService;
  }

  public async Task<RegisterResponse> Handle(
      RegisterCommand request,
      CancellationToken cancellationToken)
  {
    var emailExists = await _uow.Users.IsEmailTakenAsync(request.Email, cancellationToken);

    if (emailExists)
      throw new ValidationException(new Dictionary<string, string[]>
            {
                { "email", ["Email is already taken."] }
            });

    // Use the part before @ as default display name
    var displayName = request.Email.Split('@')[0];
    var verificationToken = GenerateToken();

    var user = new User
    {
      Email = request.Email.ToLower().Trim(),
      PasswordHash = _passwordService.HashPassword(request.Password),
      DisplayName = displayName,
      IsActive = true,
      EmailVerified = false,
      EmailVerificationToken = verificationToken,
      EmailVerificationExpiresAt = DateTime.UtcNow.AddHours(24)
    };

    await _uow.Users.AddAsync(user, cancellationToken);
    await _uow.SaveChangesAsync(cancellationToken);

    await _emailService.SendVerificationEmailAsync(
        user.Email, user.DisplayName, verificationToken, cancellationToken);

    return new RegisterResponse
    {
      UserId = user.Id,
      Email = user.Email,
      DisplayName = user.DisplayName,
      Message = "Registration successful. Please check your email to verify your account."
    };
  }

  private static string GenerateToken()
      => Convert.ToBase64String(Guid.NewGuid().ToByteArray())
          .Replace("+", "-").Replace("/", "_").Replace("=", "");
}