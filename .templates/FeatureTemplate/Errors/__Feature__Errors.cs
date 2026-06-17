using TransfersApi.Helpers.Responses;
namespace TransfersApi.App.__Feature__s.Errors;

public static class __Feature__Errors
{
  public static readonly Error NotFound = new(
    code: "__Feature__.NotFound",
    messageAr: "العنصر غير موجود",
    messageEn: "__Feature__ not found",
    type: ErrorType.NotFound
  );

  public static readonly Error AlreadyExists = new(
    code: "__Feature__.AlreadyExists",
    messageAr: "العنصر موجود بالفعل",
    messageEn: "__Feature__ already exists",
    type: ErrorType.Validation
  );
}