
using Serilog;

namespace LapisApi.Extensions;

public static class LoggingExtensions
{
  public static void AddCustomSerilog(this WebApplicationBuilder builder)
  {
    Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Error()
      .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
      )
      .CreateLogger();

    builder.Host.UseSerilog();
  }
}