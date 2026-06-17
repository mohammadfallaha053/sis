using LapisApi.App.Auth.Enums;
using LapisApi.App.Users.Dto.Request.Commands;
namespace LapisApi.Shared.Services;

public interface IEmailService
{
  Task SendEmailAsync(string to, string subject, string htmlBody);
  Task SendOtpEmail(string to, OtpPurposeEnum purpose, string code, string lang);
  Task SendContactUsEmailAsync(ContactUsCommand command, string recipientEmail, string lang = "ar");
}