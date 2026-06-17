namespace LapisApi.Helpers.Security;

public interface IAttemptTrackerService
{
  Task<bool> IsLockedAsync(string key);
  Task RegisterAttemptAsync(string key);
  Task ResetAttemptsAsync(string key);
  TimeSpan GetRemainingLockoutTime(string key);

  Task<bool> IsLimitedAsync(string key, int maxAttempts, TimeSpan window, TimeSpan lockoutDuration);

}