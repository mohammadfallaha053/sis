using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LapisApi.App.Users.Model;
using LapisApi.Data;

namespace LapisApi.Extensions;

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