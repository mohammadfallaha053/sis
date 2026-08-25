using LapisApi.Helpers.Responses;
namespace SisApi.App.Orders.Errors;

public static class OrdersErrors
{
  public static readonly Error NotFound = new(
    code: "Orders.NotFound",
    messageAr: "العنصر غير موجود",
    messageEn: "Orders not found",
    type: ErrorType.NotFound
  );

  public static readonly Error AlreadyExists = new(
    code: "Orders.AlreadyExists",
    messageAr: "العنصر موجود بالفعل",
    messageEn: "Orders already exists",
    type: ErrorType.Validation
  );
  
  
  public static readonly Error ItemsRequired = new(
    code: "Orders.ItemsRequired",
    messageAr: "يجب تحديد مادة واحدة على الأقل في الطلب",
    messageEn: "At least one item type must be selected",
    type: ErrorType.Validation
  );

  public static readonly Error PickupLocationRequired = new(
    code: "Orders.PickupLocationRequired",
    messageAr: "يجب تحديد موقع الاستلام قبل إنشاء الطلب",
    messageEn: "Pickup location must be specified before creating the order",
    type: ErrorType.Validation
  );
  
  public static readonly Error CannotAssignAtCurrentStatus = new(
    code: "Orders.CannotAssignAtCurrentStatus",
    messageAr: "لا يمكن تعيين موظف للطلب في حالته الحالية",
    messageEn: "An employee cannot be assigned to the order in its current status",
    type: ErrorType.Validation
  );

  public static readonly Error EmployeeMustBelongToOrderCenter = new(
    code: "Orders.EmployeeMustBelongToOrderCenter",
    messageAr: "الموظف المحدد لا يتبع للمركز المسؤول عن الطلب",
    messageEn: "The selected employee does not belong to the order service center",
    type: ErrorType.Validation
  );

  public static readonly Error EmployeeRequired = new(
    code: "Orders.EmployeeRequired",
    messageAr: "يجب تحديد الموظف",
    messageEn: "Employee is required",
    type: ErrorType.Validation
  );
  
  public static readonly Error CannotStartAtCurrentStatus = new(
    code: "Orders.CannotStartAtCurrentStatus",
    messageAr: "لا يمكن بدء الطلب في حالته الحالية",
    messageEn: "The order cannot be started in its current status",
    type: ErrorType.Validation
  );

  public static readonly Error NotAssignedToCurrentEmployee = new(
    code: "Orders.NotAssignedToCurrentEmployee",
    messageAr: "هذا الطلب غير معين للموظف الحالي",
    messageEn: "This order is not assigned to the current employee",
    type: ErrorType.Validation
  );
  
  public static readonly Error CannotCompleteAtCurrentStatus = new(
    code: "Orders.CannotCompleteAtCurrentStatus",
    messageAr: "لا يمكن إنهاء الطلب في حالته الحالية",
    messageEn: "The order cannot be completed in its current status",
    type: ErrorType.Validation
  );

  public static readonly Error CompleteItemsMismatch = new(
    code: "Orders.CompleteItemsMismatch",
    messageAr: "يجب إرسال أوزان جميع عناصر الطلب فقط",
    messageEn: "Weights must be provided for all and only the order items",
    type: ErrorType.Validation
  );

  public static readonly Error InvalidItemWeight = new(
    code: "Orders.InvalidItemWeight",
    messageAr: "وزن المادة يجب أن يكون أكبر من صفر",
    messageEn: "Item weight must be greater than zero",
    type: ErrorType.Validation
  );
  
  public static readonly Error CannotCancelAtCurrentStatus = new(
    code: "Orders.CannotCancelAtCurrentStatus",
    messageAr: "لا يمكن إلغاء الطلب في حالته الحالية",
    messageEn: "The order cannot be cancelled in its current status",
    type: ErrorType.Validation
  );
  
  public static readonly Error CancellationReasonRequired = new(
    code: "Orders.CancellationReasonRequired",
    messageAr: "يجب إدخال سبب إلغاء الطلب",
    messageEn: "Cancellation reason is required",
    type: ErrorType.Validation
  );
}