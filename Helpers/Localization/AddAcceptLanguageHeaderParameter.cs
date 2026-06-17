using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace LapisApi.Helpers.Localization;

public class AddAcceptLanguageHeaderParameter : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    operation.Parameters ??= new List<OpenApiParameter>();

    // Add Accept-Language header to all endpoints
    operation.Parameters.Add(new OpenApiParameter
      {
        Name = "Accept-Language",
        In = ParameterLocation.Header,
        Description = "Specify the language (e.g. en, ar, ku)",
        Required = false,
        Schema = new OpenApiSchema
        {
          Type = "string"
        }
      }
    );
  }
}