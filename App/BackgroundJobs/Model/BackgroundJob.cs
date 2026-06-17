using LapisApi.App.BackgroundJobs.Enums;
namespace LapisApi.App.BackgroundJobs.Model
{
  public class BackgroundJob
  {
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string JobType { get; set; } // مثل: "SendEmailAfterTransaction", "UpdateTransactionStatus"
    public string PayloadJson { get; set; } // سيتم تخزين كلاس الحدث كسلسلة JSON
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public int RetryCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastAttemptAt { get; set; }
    public string? ErrorMessage { get; set; }
  }
}