using LapisApi.Helpers.Responses;

namespace SisApi.App.Products.Errors;

public static class ProductsErrors
{
  public static readonly Error NotFound = new(
    code: "Products.NotFound",
    messageAr: "المنتج غير موجود",
    messageEn: "Product not found",
    type: ErrorType.NotFound
  );

  public static readonly Error Inactive = new(
    code: "Products.Inactive",
    messageAr: "المنتج غير فعال",
    messageEn: "Product is inactive",
    type: ErrorType.Validation
  );

  public static readonly Error OutOfStock = new(
    code: "Products.OutOfStock",
    messageAr: "الكمية المطلوبة غير متوفرة",
    messageEn: "The requested quantity is not available",
    type: ErrorType.Validation
  );

  public static readonly Error InsufficientPoints = new(
    code: "Products.InsufficientPoints",
    messageAr: "رصيد النقاط غير كافٍ لشراء المنتج",
    messageEn: "Insufficient points balance to purchase the product",
    type: ErrorType.Validation
  );
}
