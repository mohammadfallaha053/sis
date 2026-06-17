namespace SisApi.App.BackgroundJobs.Dto.Response;

public class BackgroundJobResponse
{
  public string Id { get; set; }
  public string JobType { get; set; } // مثل: "SendEmailAfterTransaction", "UpdateTransactionStatus"
  public string PayloadJson { get; set; }
  public string Status { get; set; }
  public int RetryCount { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? LastAttemptAt { get; set; }
  public string? ErrorMessage { get; set; }
}