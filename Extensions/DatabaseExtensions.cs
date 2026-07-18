using Microsoft.EntityFrameworkCore;
using SisApi.Data;
namespace SisApi.Extensions;

public static class DatabaseExtensions
{
  public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
  {
    services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(config.GetConnectionString("Default")));

    return services;
  }
}