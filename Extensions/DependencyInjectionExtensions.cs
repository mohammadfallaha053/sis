using LapisApi.App.BackgroundJobs;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.Helpers.Security;
using LapisApi.Middleware;
using LapisApi.Shared.EmailTemplates;
using LapisApi.Shared.Services;
using SisApi.App.Users.Interfaces;
using SisApi.Data.Interfaces;
using SisApi.Data.Repository;
using SisApi.Shared.Providers;
namespace LapisApi.Extensions;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCustomServices(this IServiceCollection services)
  {
    services.Scan(scan => scan
      .FromAssemblyOf<IUserService>()
      .AddClasses(c => c.Where(t => t.Name.EndsWith("Service")))
      .AsMatchingInterface()
      .WithScopedLifetime()
    );

    services.Scan(scan => scan
      .FromAssemblyOf<IBackgroundJobHandler>()
      .AddClasses(c => c.AssignableTo<IBackgroundJobHandler>())
      .AsImplementedInterfaces()
      .WithScopedLifetime()
    );

    services.AddTransient<BotProtectionMiddleware>();
    services.AddTransient<RateLimitMiddleware>(); 

    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<JwtProvider>();
    services.AddScoped<BackgroundJobProcessor>();
    services.AddTransient<EmailTemplateOtpBuilder>();
    services.AddTransient<EmailContactUsBuilder>();
    services.AddScoped<ISecretCodeSignatureHelper, SecretCodeSignatureHelper>();

    return services;
  }
}