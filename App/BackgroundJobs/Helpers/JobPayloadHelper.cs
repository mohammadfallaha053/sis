using System.Text.Json;
using LapisApi.App.BackgroundJobs.Interfaces;

namespace LapisApi.App.BackgroundJobs.Helpers;

public static class JobPayloadHelper
{
  public static T Deserialize<T>(string json) where T : IBackgroundJobPayload
  {
    try
    {
      return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      }) ?? throw new Exception($"Failed to deserialize payload into {typeof(T).Name}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"[PayloadError] Deserialization failed: {ex.Message}");
      throw;
    }
  }
}
