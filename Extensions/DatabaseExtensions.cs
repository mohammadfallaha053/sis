using Microsoft.EntityFrameworkCore;
using LapisApi.Data;
namespace LapisApi.Extensions;

public static class DatabaseExtensions
{
  public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
  {
    services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(config.GetConnectionString("Default")));

    return services;
  }
}