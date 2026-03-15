namespace VocabToeic.Application.Features.Auth.DTOs;

/// <summary>
/// FE gửi lên ID Token sau khi user đăng nhập Google thành công.
/// ID Token là JWT do Google ký — backend verify bằng Google.Apis.Auth.
/// </summary>
public record GoogleLoginRequest(string IdToken);