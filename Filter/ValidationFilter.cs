using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using LapisApi.Helpers.Responses;

public class ValidationFilter : IActionFilter
{
  public void OnActionExecuting(ActionExecutingContext context)
  {
    if (!context.ModelState.IsValid)
    {
      var firstError = context.ModelState
        .Where(x => x.Value.Errors.Any())
        .Select(x => new
        {
          Field = x.Key,
          Error = x.Value.Errors.First().ErrorMessage
        })
        .FirstOrDefault();

      var appResponse = new AppResponse<object>
      {
        IsSuccess = false,
        Error = new ErrorDetails
        {
          Code = "ValidationError",
          Message = firstError?.Error ?? "Validation failed"
        }
      };

      context.Result = new BadRequestObjectResult(appResponse);
    }
  }

  public void OnActionExecuted(ActionExecutedContext context) { }
}