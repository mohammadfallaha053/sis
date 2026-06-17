using Microsoft.AspNetCore.Identity;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Users.Model;
using LapisApi.MyEnum;
namespace LapisApi.Services.Seed;

public static class SeederService
{
  public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
  {
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // 1. أنشئ الأدوار المطلوبة
    foreach (var role in Enum.GetNames(typeof(RoleEnum)))
    {
      if (!await roleManager.RoleExistsAsync(role))
      {
        await roleManager.CreateAsync(new IdentityRole(role));
      }
    }

    // 2. إعداد معلومات المستخدم الإداري
    var adminEmail = "admin@admin.com";
    var adminPassword = "053053";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
      var user = new ApplicationUser
      {
        UserName = adminEmail,
        Email = adminEmail,
        EmailConfirmed = true,
        FirstName = "Admin",
        LastName = "Abokhaled",
        CreatedAt = DateTime.UtcNow,
        IsActive = true,
        Role = RoleEnum.Admin
      };

      var result = await userManager.CreateAsync(user, adminPassword);
      if (result.Succeeded)
      {
        await userManager.AddToRoleAsync(user, nameof(RoleEnum.Admin));
      }
    }
  }
}