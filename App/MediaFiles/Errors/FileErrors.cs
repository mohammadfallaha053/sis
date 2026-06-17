using LapisApi.Helpers.Responses;

public static class FileErrors
{
  public static readonly Error FileNotFound = new(
    code: "File.NotFound",
    messageAr: "الملف غير موجود",
    messageEn: "File not found",
    type: ErrorType.NotFound
  );

  public static readonly Error FileAlreadyAttached = new(
    code: "File.AlreadyAttached",
    messageAr: "الملف مرتبط مسبقًا",
    messageEn: "File is already attached",
    type: ErrorType.Validation
  );

  public static readonly Error UploadInvalid = new(
    code: "File.UploadInvalid",
    messageAr: "ملف غير صالح للرفع",
    messageEn: "Invalid file upload",
    type: ErrorType.Validation
  );
  
  public static readonly Error UnsupportedFileType = new(
    code: "File.UnsupportedType",
    messageAr: "نوع الملف غير مدعوم",
    messageEn: "Unsupported file type",
    type: ErrorType.Validation
  );

  public static readonly Error FileTooLarge = new(
    code: "File.TooLarge",
    messageAr: "حجم الملف يتجاوز الحد المسموح",
    messageEn: "File size exceeds the allowed limit",
    type: ErrorType.Validation
  );
  
  public static readonly Error MaxLimitFileNumber = new(
    code: "File.MaxLimitFileNumber",
    messageAr: "تجاوزت الحدى الاقصى لعدد الملفات",
    messageEn: "Max Limit File Number",
    type: ErrorType.Validation
  );

  public static readonly Error FileMoveFailed = new(
    code: "File.MoveFailed",
    messageAr: "فشل في نقل الملف",
    messageEn: "Failed to move file",
    type: ErrorType.Validation
  );

  public static readonly Error SourceFileMissing = new(
    code: "File.SourceMissing",
    messageAr: "الملف غير موجود على القرص",
    messageEn: "Source file not found on disk",
    type: ErrorType.Validation
  );

  public static readonly Error FileDeleteFailed = new(
    code: "File.DeleteFailed",
    messageAr: "فشل حذف الملف من القرص",
    messageEn: "Failed to delete file from disk",
    type: ErrorType.Validation
  );
  
  
}