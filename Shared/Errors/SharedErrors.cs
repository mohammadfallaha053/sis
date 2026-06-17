using LapisApi.Helpers.Responses;
namespace LapisApi.Shared.Errors;

public static class SharedErrors
{
  public static readonly Error CreateFailed = new(
    code: "SharedErrors.CreateFailed",
    messageAr: "فشل الإنشاء",
    messageEn: "Create failed",
    type: ErrorType.Validation
  );
  
  public static readonly Error UpdateFailed = new(
    code: "SharedErrors.UpdateFailed",
    messageAr: "فشل التحديث",
    messageEn: "Update failed",
    type: ErrorType.Validation
  );

  public static readonly Error ServerError = new(
    code: "SharedErrors.ServerError",
    messageAr: "حدث خطأ داخلي في الخادم",
    messageEn: "Internal server error",
    type: ErrorType.ServerError
  );

  public static readonly Error Unknown = new(
    code: "SharedErrors.UnknownError",
    messageAr: "حدث خطأ غير معروف",
    messageEn: "An unknown error occurred",
    type: ErrorType.ServerError
  );
}