using Microsoft.AspNetCore.Mvc;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;

public static class ResultExtensions
{
  public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
  {
    var acceptLanguage = controller.Request.Headers.AcceptLanguage.FirstOrDefault();
    var language = (acceptLanguage?.Contains("ar") ?? false) ? AppLanguageEnum.Ar : AppLanguageEnum.En;

    if (result.IsSuccess)
    {
      return controller.Ok(
        new AppResponse<T>
        {
          IsSuccess = true,
          Data = result.Data,
          Paging = result.Paging
        }
      );
    }

    var appError = ErrorDetails.FromError(result.Error, language);

    var statusCode = result.Error.Type switch
    {
      ErrorType.NotFound => StatusCodes.Status404NotFound,
      ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
      ErrorType.ServerError => StatusCodes.Status500InternalServerError,
      _ => StatusCodes.Status400BadRequest
    };

    return controller.StatusCode(statusCode, new AppResponse<T>
      {
        IsSuccess = false,
        Error = appError
      }
    );
  }
}