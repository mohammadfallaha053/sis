using LapisApi.Helpers.Responses;
namespace LapisApi.App.Auth.Errors;

public static class AuthErrors
{
  public static readonly Error Unauthorized = new(
    code: "Auth.Unauthorized",
    messageAr: "غير مصرح لك",
    messageEn: "Unauthorized",
    type: ErrorType.Unauthorized
  );

  public static readonly Error Forbidden = new(
    code: "Auth.Forbidden",
    messageAr: "ممنوع الوصول",
    messageEn: "Forbidden",
    type: ErrorType.Unauthorized
  );

  public static readonly Error InvalidVerificationCode = new(
    code: "Auth.InvalidVerificationCode",
    messageAr: "خطاء بتأكيد الكود",
    messageEn: "Invalid Verification Code",
    type: ErrorType.Validation
  );

  public static readonly Error EmailNotConfirmed = new(
    code: "Auth.EmailNotConfirmed",
    messageAr: "الأيميل ليس مؤكد",
    messageEn: "Email Not Confirmed",
    type: ErrorType.Validation
  );

  public static readonly Error RequireVerificationCode = new(
    code: "Auth.RequireVerificationCode",
    messageAr: "كود التحقق مطلوب",
    messageEn: "Require Verification Code",
    type: ErrorType.Validation
  );

  public static readonly Error TooManyRequests = new(
    code: "Auth.TooManyRequests",
    messageAr: "عدد كبير من المحاولات، يرجى المحاولة لاحقاً",
    messageEn: "Too many requests, please try again later",
    type: ErrorType.Validation
  );
  
  public static Error TooManyRequestsWithTime(int remainingSeconds) => new(
    code: "Auth.TooManyRequests",
    messageAr: $"عدد المحاولات تجاوز الحد المسموح. الرجاء المحاولة بعد {remainingSeconds/60} دقيقة.",
    messageEn: $"Too many attempts. Please try again after {remainingSeconds/60} minutes.",
    type: ErrorType.Validation
  );
}