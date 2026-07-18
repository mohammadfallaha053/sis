using Microsoft.AspNetCore.Identity;
using SisApi.App.Users.Model;
using SisApi.Data;
namespace SisApi.Extensions;

public static class IdentityServiceExtensions
{
  public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
  {
    services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
          options.Password.RequiredLength = 6;
          options.Password.RequireNonAlphanumeric = false;
          options.Password.RequireDigit = false;
          options.Password.RequireUppercase = false;
          options.Password.RequireLowercase = false;
        }
      )
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();

    return services;
  }
}