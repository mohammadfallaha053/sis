namespace LapisApi.OptionConfigurations;

public class AttemptTrackerSettings
{
  public int MaxAttempts { get; set; } = 3;
  public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(10);
}