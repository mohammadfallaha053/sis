using LapisApi.Helpers.Responses;
namespace LapisApi.App.Users.Errors;

public static class UserErrors
{
  public static readonly Error PasswordResetFailed = new(
    code: "User.PasswordResetFailed",
    messageAr: "فشل إعادة تهية كلمة المرور",
    messageEn: "Password reset failed",
    type: ErrorType.Validation
  );

  public static readonly Error PasswordChangeFailed = new(
    code: "User.PasswordChangeFailed",
    messageAr: "فشل تغير كلمة المرور",
    messageEn: "Password change failed",
    type: ErrorType.Validation
  );

  public static readonly Error EmailOrPasswordIncorrect = new(
    code: "User.EmailOrPasswordIncorrect",
    messageAr: "الايميل او كلمة المرور غير صحيحة",
    messageEn: "Email or password is incorrect",
    type: ErrorType.Validation
  );

  public static readonly Error EmailAlreadyUsed = new(
    code: "User.EmailAlreadyUsed",
    messageAr: "الايميل مستخدم من قبل",
    messageEn: "The email is already used",
    type: ErrorType.Validation
  );

  public static readonly Error EmailConfirmationFailed = new(
    code: "User.EmailConfirmationFailed",
    messageAr: "فشل تأكيد البريد الإلكتروني",
    messageEn: "Email confirmation failed",
    type: ErrorType.Validation
  );

  public static readonly Error NotFound = new(
    code: "User.NotFound",
    messageAr: "المستخدم غير موجود",
    messageEn: "User not found",
    type: ErrorType.NotFound
  );

  public static readonly Error AgentNotFound = new(
    code: "Agent.NotFound",
    messageAr: "العميل غير موجود",
    messageEn: "Agent not found",
    type: ErrorType.NotFound
  );


  public static readonly Error ClientNotFound = new(
    code: "Client.NotFound",
    messageAr: "المرسل غير موجود",
    messageEn: "Client not found",
    type: ErrorType.NotFound
  );


  public static readonly Error AgentAlreadyDisabled = new(
    code: "Agent.AlreadyDisabled",
    messageAr: "العميل غير موجود",
    messageEn: "Agent already disabled",
    type: ErrorType.NotFound
  );

  public static Error AccountLocked(TimeSpan wait) => new(
    code: "Auth.Locked",
    messageAr: $"تم قفل الحساب مؤقتاً. يرجى المحاولة بعد {wait.Minutes} دقيقة.",
    messageEn: $"Account is temporarily locked. Try again in {wait.Minutes} minutes.",
    type: ErrorType.Unauthorized
  );

  public static Error AccountDisabled => new(
    code: "Auth.Locked",
    messageAr: "تم تعطيل من قبل الأدارة الحساب لأسباب أمنية",
    messageEn: "The account has been disabled by the administration for security reasons.",
    type: ErrorType.Unauthorized
  );
}