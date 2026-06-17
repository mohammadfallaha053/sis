using LapisApi.Helpers.Responses;
namespace LapisApi.App.Centers.Errors;

public static class CenterErrors
{
  public static readonly Error NotFound = new(
    code: "Center.NotFound",
    messageAr: "المركز غير موجود",
    messageEn: "Center not found",
    type: ErrorType.Unauthorized
  );
  
  public static readonly Error AlreadyExists = new(
    code: "Center.AlreadyExists",
    messageAr: "المركز موجود بالفعل",
    messageEn: "Center already exists",
    type: ErrorType.NotFound
  );
  
  public static readonly Error LastTemporaryPaymentWhileStatusPending = new(
    code: "Center.LastTemporaryPaymentWhileStatusPending",
    messageAr: " الدفعه الاخيرة مازالة في حالة إلانتظار",
    messageEn: "Last Temporary Payment While Status Pending",
    type: ErrorType.NotFound
  );
  
  
  
  
}