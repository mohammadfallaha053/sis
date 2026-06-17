using LapisApi.Helpers.Responses;
namespace LapisApi.App.Settings.Errors;

public static class SettingErrors
{
  public static readonly Error NotFound = new(
    code: "Setting.NotFound",
    messageAr: "المركز غير موجود",
    messageEn: "Setting not found",
    type: ErrorType.NotFound
  );
}