using LapisApi.Helpers.Responses;
namespace SisApi.App.ItemTypes.Errors;

public static class ItemTypesErrors
{
  public static readonly Error NotFound = new(
    code: "ItemTypes.NotFound",
    messageAr: "العنصر غير موجود",
    messageEn: "ItemTypes not found",
    type: ErrorType.NotFound
  );

  public static readonly Error AlreadyExists = new(
    code: "ItemTypes.AlreadyExists",
    messageAr: "العنصر موجود بالفعل",
    messageEn: "ItemTypes already exists",
    type: ErrorType.Validation
  );
}