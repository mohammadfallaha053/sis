using LapisApi.App.BackgroundJobs.Enums;
using LapisApi.App.BackgroundJobs.Helpers;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.BackgroundJobs.Jobs.Payloads;
using LapisApi.App.Users.Dto.Request.Commands;
using LapisApi.Data.Interfaces;
using LapisApi.Helpers.Security;
using LapisApi.Shared.Services;
namespace LapisApi.App.BackgroundJobs.Jobs.Handlers;

public class SendEmailAfterContactUsHandler : IBackgroundJobHandler
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IEmailService _emailService;
  private readonly ISecretCodeSignatureHelper _signatureHelper;
  public string JobType => BackgroundJobTypes.SendEmailAfterContactUs;

  public SendEmailAfterContactUsHandler(
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    ISecretCodeSignatureHelper signatureHelper
  )
  {
    _unitOfWork = unitOfWork;
    _emailService = emailService;
    _signatureHelper = signatureHelper;
  }
  public async Task HandleAsync(string payloadJson)
  {
    var payload = JobPayloadHelper.Deserialize<SendEmailAfterContactUsPayload>(payloadJson)!;

    string recipientEmail;
    string lang = "ar"; // يمكنك جلب اللغة من الـ request لاحقًا

    if (payload.IsAgent)
    {
      recipientEmail = "support@buzcash.com";
    }
    else
    {
      recipientEmail = "info@buzcash.com";
    }

    var request =
      new ContactUsCommand
      {
        Email = payload.Email,
        FullName = payload.FullName,
        PhoneNumber = payload.PhoneNumber,
        JobType = payload.JobType,
        Message = payload.Message,
        IsAgent = payload.IsAgent
      };
    await _emailService.SendContactUsEmailAsync(request, recipientEmail, lang);
  }
}