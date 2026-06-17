using LapisApi.App.Auth.Dto;
using LapisApi.Dto.Auth;
using LapisApi.Dto.Client;
namespace LapisApi.Interfaces.Auth
{
  public interface IAuthService
  {
    Task<Result<ClientRegisterResponse>> RegisterAsync(
      RegisterRequest model
    );
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);

    Task<Result<object>> ConfirmEmailAsync(ConfirmEmailRequest request, string acceptLanguage);

    Task<Result<object>> ResetPasswordByOtpAsync(ResetPasswordRequest request);
    // Task<Result<object>> SendEmailConfirmationAsync(string email, string acceptLanguage);
    //
    // Task<Result<object>> ForgotPasswordAsync(string email, string acceptLanguage);
    //
    // Task<Result<object>> ResetPasswordAsync(string email, string code, string newPassword);
  
    Task<Result<bool>> ToggleTwoFactorAsync(ToggleTwoFactorRequest request);
  } 
}