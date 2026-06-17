using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Users.Dto.Request.Commands;
using LapisApi.App.Users.Model;
using LapisApi.Helpers;
using LapisApi.OptionConfigurations;
using LapisApi.Shared.EmailTemplates;
namespace LapisApi.Shared.Services;

public class EmailService : IEmailService
{
  private readonly EmailOptionSettings _optionSettings;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly EmailTemplateOtpBuilder _otpBuilder;
  private readonly EmailContactUsBuilder _emailContactUsBuilder;
  public EmailService(
    IOptions<EmailOptionSettings> options,
    UserManager<ApplicationUser> userManager,
    EmailTemplateOtpBuilder otpBuilder,

    EmailContactUsBuilder emailContactUsBuilder
  )
  {
    _optionSettings = options.Value;
    _userManager = userManager;
    _otpBuilder = otpBuilder;
    _emailContactUsBuilder = emailContactUsBuilder;
  }


  public async Task SendEmailAsync(string to, string subject, string htmlBody)
  {
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress(_optionSettings.ClientName, _optionSettings.ClientEmail));
    message.To.Add(MailboxAddress.Parse(to));
    message.Subject = subject;

    var builder = new BodyBuilder
    {
      HtmlBody = htmlBody
    };
    message.Body = builder.ToMessageBody();

    using var client = new SmtpClient();
    await client.ConnectAsync(_optionSettings.Server, _optionSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
    await client.AuthenticateAsync(_optionSettings.Username, _optionSettings.Password);
    await client.SendAsync(message);
    await client.DisconnectAsync(true);
  }
  public async Task SendOtpEmail(string to, OtpPurposeEnum purpose, string code, string lang)
  {
    var (subject, html) = _otpBuilder.Build(purpose, code, lang);

    if (string.IsNullOrEmpty(to))
    {
      throw new Exception("Client email not found");
    }

    await SendEmailAsync(to, subject, html);
  }
  
  public async Task SendContactUsEmailAsync(ContactUsCommand command, string recipientEmail, string lang = "ar")
  {
    var (subject, html) =
      _emailContactUsBuilder
        .Build(
          command,
          command.IsAgent,
          lang
        );

    await SendEmailAsync(
      to: recipientEmail,
      subject: subject,
      htmlBody: html
    );
  }
}