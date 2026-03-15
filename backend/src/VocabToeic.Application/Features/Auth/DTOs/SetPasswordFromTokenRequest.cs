namespace VocabToeic.Application.Features.Auth.DTOs;

public record SetPasswordFromTokenRequest(string NewPassword, string ConfirmPassword);