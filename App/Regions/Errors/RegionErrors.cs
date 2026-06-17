using LapisApi.Helpers.Responses;
namespace LapisApi.App.Regions.Errors;

public static class RegionErrors
{
  public static readonly Error NotFound = new(
    code: "Region.NotFound",
    messageAr: "المدينة غير موجودة",
    messageEn: "Region not found",
    type: ErrorType.NotFound
  );
}