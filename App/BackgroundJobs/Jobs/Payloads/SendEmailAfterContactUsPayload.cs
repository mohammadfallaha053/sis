using LapisApi.App.BackgroundJobs.Interfaces;
namespace LapisApi.App.BackgroundJobs.Jobs.Payloads;

public class SendEmailAfterContactUsPayload : IBackgroundJobPayload
{
  public string Email { get; set; }
  public string FullName { get; set; }
  public string PhoneNumber { get; set; }
  public string? JobType { get; set; } = null;
  public string Message { get; set; }
  public bool IsAgent { get; set; }
}