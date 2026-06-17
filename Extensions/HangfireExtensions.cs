using Hangfire;
namespace LapisApi.Extensions;

public static class HangfireExtensions
{
  public static IServiceCollection AddCustomHangfire(this IServiceCollection services, IConfiguration config)
  {
    services.AddHangfire(x =>
      x.UseSqlServerStorage(config.GetConnectionString("Default")));

    services.AddHangfireServer();

    return services;
  }
}