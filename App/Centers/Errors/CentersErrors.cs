using LapisApi.Helpers.Responses;
namespace SisApi.App.Centers.Errors;

public static class CentersErrors
{
  public static readonly Error NotFound = new(
    code: "Centers.NotFound",
    messageAr: "المركز غير موجود",
    messageEn: "Center not found",
    type: ErrorType.NotFound
  );

  public static readonly Error AlreadyExists = new(
    code: "Centers.AlreadyExists",
    messageAr: "المركز موجود بالفعل",
    messageEn: "Center already exists",
    type: ErrorType.Validation
  );

  public static readonly Error NoCenterForThisManagerYet = new(
    code: "Centers.NoCenterForThisManagerYet",
    messageAr: "لم يتم تحديد مركز لهذا المدير بعد",
    messageEn: "No center has been assigned to this manager yet",
    type: ErrorType.NotFound
  );

  public static readonly Error UserIsNotManager = new(
    code: "Centers.UserIsNotManager",
    messageAr: "المستخدم المحدد لا يمتلك صلاحية مدير",
    messageEn: "The selected user does not have the Manager role",
    type: ErrorType.Validation
  );

  public static readonly Error ManagerBelongsToAnotherCenter = new(
    code: "Centers.ManagerBelongsToAnotherCenter",
    messageAr: "المدير المحدد مرتبط بمركز آخر",
    messageEn: "The selected manager belongs to another center",
    type: ErrorType.Validation
  );

  public static readonly Error ManagerAlreadyAssigned = new(
    code: "Centers.ManagerAlreadyAssigned",
    messageAr: "المدير المحدد معيّن بالفعل كمدير لمركز آخر",
    messageEn: "The selected manager is already assigned to another center",
    type: ErrorType.Validation
  );

  public static readonly Error ManagerMustBelongToCenter = new(
    code: "Centers.ManagerMustBelongToCenter",
    messageAr: "يجب أن يكون المدير تابعًا لنفس المركز",
    messageEn: "The manager must belong to the same center",
    type: ErrorType.Validation
  );

  public static readonly Error ManagerNotAssigned = new(
    code: "Centers.ManagerNotAssigned",
    messageAr: "لا يوجد مدير معيّن لهذا المركز",
    messageEn: "No manager is assigned to this center",
    type: ErrorType.NotFound
  );

  public static readonly Error ManagerAlreadyAssignedToThisCenter = new(
    code: "Centers.ManagerAlreadyAssignedToThisCenter",
    messageAr: "هذا المستخدم معيّن بالفعل كمدير لهذا المركز",
    messageEn: "This user is already assigned as the manager of this center",
    type: ErrorType.Validation
  );

  public static readonly Error CenterIsRequiredForEmployee = new(
    code: "Centers.CenterIsRequiredForEmployee",
    messageAr: "يجب تحديد مركز للموظف",
    messageEn: "A center must be specified for the employee",
    type: ErrorType.Validation
  );

  public static readonly Error CenterIsRequiredForManager = new(
    code: "Centers.CenterIsRequiredForManager",
    messageAr: "يجب تحديد مركز للمدير",
    messageEn: "A center must be specified for the manager",
    type: ErrorType.Validation
  );

  public static readonly Error CannotDeleteCenterWithEmployees = new(
    code: "Centers.CannotDeleteCenterWithEmployees",
    messageAr: "لا يمكن حذف المركز لوجود موظفين مرتبطين به",
    messageEn: "The center cannot be deleted because it has associated employees",
    type: ErrorType.Validation
  );

  public static readonly Error CannotDeleteCenterWithManager = new(
    code: "Centers.CannotDeleteCenterWithManager",
    messageAr: "لا يمكن حذف المركز قبل إزالة المدير المرتبط به",
    messageEn: "The center cannot be deleted before removing its assigned manager",
    type: ErrorType.Validation
  );
  
  public static readonly Error UserIsNotCenterManager = new(
    code: "Centers.UserIsNotCenterManager",
    messageAr: "المستخدم الحالي ليس المدير المعيّن لهذا المركز",
    messageEn: "The current user is not the assigned manager of this center",
    type: ErrorType.Validation
  );
  
  
}