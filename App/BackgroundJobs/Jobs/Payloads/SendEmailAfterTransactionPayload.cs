using LapisApi.App.BackgroundJobs.Interfaces;
namespace LapisApi.App.BackgroundJobs.Jobs.Payloads;

public class SendEmailAfterTransactionPayload : IBackgroundJobPayload
{
  public string TransactionId { get; set; }
  public bool IsDetails { get; set; } = false;
}