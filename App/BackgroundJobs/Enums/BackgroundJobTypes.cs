namespace LapisApi.App.BackgroundJobs.Enums;

public static class BackgroundJobTypes
{
  public const string SendEmailAfterTransaction = "SendEmailAfterTransaction";
  public const string SendEmailAfterContactUs = "SendEmailAfterContactUs";
  public const string UpdateTransactionAutoApproved = "UpdateTransactionAutoApproved";
  public const string CleanBackgroundJobHandler = "CleanBackgroundJob";
}