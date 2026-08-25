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
    messageAr: "يجب تحديد المنطقة",
    messageEn: "Region is required",
    type: ErrorType.Validation
  );

  public static readonly Error Inactive = new(
    code: "Region.Inactive",
    messageAr: "المنطقة المحددة غير مفعلة حاليًا",
    messageEn: "The selected region is currently inactive",
    type: ErrorType.Validation
  );

  public static readonly Error HasNoServiceCenter = new(
    code: "Region.HasNoServiceCenter",
    messageAr: "لا يوجد مركز فعال يخدم المنطقة المحددة",
    messageEn: "There is no active service center for the selected region",
    type: ErrorType.Validation
  );
}