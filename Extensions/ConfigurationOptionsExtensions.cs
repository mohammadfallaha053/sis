using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LapisApi.OptionConfigurations;

namespace LapisApi.Extensions;

public static class ConfigurationOptionsExtensions
{
  public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<EmailOptionSettings>(configuration.GetSection("SmtpSettings"));
    services.Configure<PayPalOptions>(configuration.GetSection("PayPal"));
    services.Configure<PaymentRedirectUrlsOptionSettings>(configuration.GetSection("PaymentRedirectUrls"));
    services.Configure<JwtOptions>(configuration.GetSection("JWT"));
    services.Configure<FrontendSettings>(configuration.GetSection("Frontend"));
    services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));
    services.Configure<AttemptTrackerSettings>(configuration.GetSection("AttemptTrackerSettings"));

    return services;
  }
}