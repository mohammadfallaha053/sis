using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using LapisApi.OptionConfigurations;
namespace LapisApi.Helpers.Security;

public class AttemptTrackerService : IAttemptTrackerService
{
  private readonly IMemoryCache _cache;
  private readonly AttemptTrackerSettings _settings;

  public AttemptTrackerService(IMemoryCache cache, IOptions<AttemptTrackerSettings> options)
  {
    _cache = cache;
    _settings = options.Value;
  }

  public async Task<bool> IsLockedAsync(string key)
  {
    if (_cache.TryGetValue<AttemptInfo>(GetCacheKey(key), out var info))
    {
      if (info.Attempts >= _settings.MaxAttempts)
      {
        var lockUntil = info.FirstAttemptTime.Add(_settings.LockoutDuration);
        return DateTime.UtcNow < lockUntil;
      }
    }
    return false;
  }

  public async Task RegisterAttemptAsync(string key)
  {
    var cacheKey = GetCacheKey(key);

    if (!_cache.TryGetValue<AttemptInfo>(cacheKey, out var info))
    {
      info = new AttemptInfo
      {
        Attempts = 1,
        FirstAttemptTime = DateTime.UtcNow
      };
    }
    else
    {
      info.Attempts++;
    }

    _cache.Set(cacheKey, info, info.FirstAttemptTime.Add(_settings.LockoutDuration));
  }

  public async Task ResetAttemptsAsync(string key)
  {
    _cache.Remove(GetCacheKey(key));
  }

  public TimeSpan GetRemainingLockoutTime(string key)
  {
    if (_cache.TryGetValue<AttemptInfo>(GetCacheKey(key), out var info))
    {
      var lockUntil = info.FirstAttemptTime.Add(_settings.LockoutDuration);
      return lockUntil - DateTime.UtcNow;
    }
    return TimeSpan.Zero;
  }

  private string GetCacheKey(string key) => $"AttemptTracker:{key}";

  private class AttemptInfo
  {
    public int Attempts { get; set; }
    public DateTime FirstAttemptTime { get; set; }
  }
  
  public async Task<bool> IsLimitedAsync(string key, int maxAttempts, TimeSpan window, TimeSpan lockoutDuration)
  {
    var cacheKey = GetCacheKey(key);

    if (!_cache.TryGetValue<AttemptInfo>(cacheKey, out var info))
    {
      info = new AttemptInfo
      {
        Attempts = 1,
        FirstAttemptTime = DateTime.UtcNow
      };
    }
    else
    {
      // تحقق من مدة النافذة الزمنية
      if (DateTime.UtcNow - info.FirstAttemptTime <= window)
      {
        info.Attempts++;
      }
      else
      {
        // إعادة تعيين النافذة
        info.Attempts = 1;
        info.FirstAttemptTime = DateTime.UtcNow;
      }
    }

    // تحديث الكاش مع التوقيت الجديد
    _cache.Set(cacheKey, info, info.FirstAttemptTime.Add(lockoutDuration));

    return info.Attempts > maxAttempts;
  }

}
