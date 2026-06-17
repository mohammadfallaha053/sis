using LapisApi.Helpers.Responses;
namespace LapisApi.App.Cities.Errors;

public static class CityErrors
{
  public static readonly Error NotFound = new(
    code: "City.NotFound",
    messageAr: "البلد غير موجودة",
    messageEn: "City not found",
    type: ErrorType.NotFound
  );
}