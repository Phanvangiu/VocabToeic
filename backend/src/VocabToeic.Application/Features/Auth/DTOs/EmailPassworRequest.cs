namespace VocabToeic.Application.Features.Auth.DTOs;

public record VerifyEmailRequest(string Token);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Token, string NewPassword, string ConfirmPassword);

public record SetPasswordRequest(string Token, string NewPassword, string ConfirmPassword);