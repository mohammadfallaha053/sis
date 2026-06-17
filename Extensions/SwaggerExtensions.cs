using LapisApi.Helpers.Localization;
using Microsoft.OpenApi.Models;
using System.Reflection;
namespace LapisApi.Extensions;

public static class SwaggerExtensions
{
  public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
  {
    services.AddSwaggerGen(op =>
    {
      op.SwaggerDoc("v1", new OpenApiInfo
      {
        Version = "v1",
        Title = "S I S API",
        Contact = new OpenApiContact
        {
          Name = "Mohammad Fallaha  +963 956 661 418",
          Email = "mohammadfallaha053@gmail.com",
        },
      });

      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      op.IncludeXmlComments(xmlPath);

      op.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
      {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your token}"
      });

      op.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
            {
              Type = ReferenceType.SecurityScheme,
              Id = "Bearer"
            },
            Name = "Bearer",
            In = ParameterLocation.Header,
          },
          new List<string>()
        }
      });

      op.OperationFilter<AddAcceptLanguageHeaderParameter>();
    });

    return services;
  }
}