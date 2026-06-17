using JWT53.MyEnum;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LapisApi.App.Auth.Errors;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;

namespace LapisApi.Extensions;

public static class JwtAuthenticationExtensions
{
  public static IServiceCollection AddCustomJwtAuthentication(this IServiceCollection services, IConfiguration config)
  {
    services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
      options.RequireHttpsMetadata = false;
      options.SaveToken = false;

      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
      };

      options.Events = new JwtBearerEvents
      {
        OnChallenge = async context =>
        {
          context.HandleResponse(); // يمنع الرد الافتراضي

          var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault();
          var language = (acceptLanguage?.Contains("ar") ?? false) ? AppLanguageEnum.Ar : AppLanguageEnum.En;

          var error = ErrorDetails.FromError(AuthErrors.Unauthorized, language);

          var result = new AppResponse<object>
          {
            IsSuccess = false,
            Error = error
          };

          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          context.Response.ContentType = "application/json";
          await context.Response.WriteAsJsonAsync(result);
        },

        OnForbidden = async context =>
        {
          var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault();
          var language = (acceptLanguage?.Contains("ar") ?? false) ? AppLanguageEnum.Ar : AppLanguageEnum.En;

          var error = ErrorDetails.FromError(AuthErrors.Forbidden, language);

          var result = new AppResponse<object>
          {
            IsSuccess = false,
            Error = error
          };

          context.Response.StatusCode = StatusCodes.Status403Forbidden;
          context.Response.ContentType = "application/json";
          await context.Response.WriteAsJsonAsync(result);
        }
      };

    });

    return services;
  }
}
