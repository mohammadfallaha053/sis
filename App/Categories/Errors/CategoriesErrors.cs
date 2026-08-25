using LapisApi.Helpers.Responses;

namespace SisApi.App.Categories.Errors;

public static class CategoriesErrors
{
  public static readonly Error NotFound = new(
    code: "Categories.NotFound",
    messageAr: "التصنيف غير موجود",
    messageEn: "Category not found",
    type: ErrorType.NotFound
  );

  public static readonly Error Inactive = new(
    code: "Categories.Inactive",
    messageAr: "التصنيف غير فعال",
    messageEn: "Category is inactive",
    type: ErrorType.Validation
  );
}
