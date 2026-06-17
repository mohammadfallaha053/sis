using Hangfire;
using LapisApi.App.BackgroundJobs.Jobs;
namespace LapisApi.App.BackgroundJobs.Scheduler;

public static class HangfireRecurringJobs
{
  public static void RegisterRecurringJobs()
  {
    RecurringJob.AddOrUpdate<BackgroundJobProcessor>(
      "ProcessBackgroundJobs",
      methodCall: x => x.ProcessJobsAsync(),
      cronExpression: Cron.Minutely(),
      timeZone: TimeZoneInfo.Local
    );
    
    RecurringJob.AddOrUpdate<BackgroundJobProcessor>(
      "ExecuteOnHour",
      methodCall: x => x.ExecuteOnHourAsync(),
      cronExpression: Cron.Hourly(),
      timeZone: TimeZoneInfo.Local
    );

    RecurringJob.AddOrUpdate<BackgroundJobProcessor>(
      "ExecuteOnDay",
      methodCall: x => x.ExecuteOnDayAsync(),
      cronExpression: Cron.Daily(),
      timeZone: TimeZoneInfo.Local
    );
  }
}