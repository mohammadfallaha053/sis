using LapisApi.Helpers.Responses;
namespace SisApi.App.Regions.Errors;

public static class RegionErrors
{
  public static readonly Error NotFound = new(
    code: "Region.NotFound",
    messageAr: "المنطقة غير موجودة",
    messageEn: "Region not found",
    type: ErrorType.NotFound
  );

  public static readonly Error Required = new(
    code: "Region.Required",
    messageAr: "تحديد المنطقة  مطلوب",
    messageEn: "Region required",
    type: ErrorType.NotFound
  );
}