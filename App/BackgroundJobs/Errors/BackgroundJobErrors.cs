using LapisApi.Helpers.Responses;
namespace LapisApi.App.BackgroundJobs.Errors;

public static class BackgroundJobErrors
{
  public static readonly Error NotFound = new(
    code: "BackgroundJob.NotFound",
    messageAr: "كود الخصم غير موجود",
    messageEn: "BackgroundJob not found",
    type: ErrorType.NotFound
  );

  public static readonly Error AlreadyExists = new(
    code: "BackgroundJob.AlreadyExists",
    messageAr: "كود الخصم موجود بالفعل",
    messageEn: "BackgroundJob already exists",
    type: ErrorType.NotFound
  );


}