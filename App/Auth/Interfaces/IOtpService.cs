using LapisApi.App.Auth.Dto;
namespace LapisApi.App.Auth.Interfaces;

public interface IOtpService
{
  Task<Result<object>> SendOtpAsync(SendOtpRequest model, string lang = "ar");
  Task<Result<object>> VerifyOtpAsync(string email, string code);
}