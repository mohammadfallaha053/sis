using Microsoft.EntityFrameworkCore;
using LapisApi.App.Auth.Errors;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.MediaFiles.Dto;
using LapisApi.App.MediaFiles.Enums;
using LapisApi.App.MediaFiles.Interfaces;
using LapisApi.App.MediaFiles.Model;
using LapisApi.Data;
using LapisApi.Helpers.Extensions;
using LapisApi.Helpers.Security;
namespace LapisApi.App.MediaFiles.Services;

public class FileService : IFileService
{
  private readonly ApplicationDbContext _context;
  private readonly IWebHostEnvironment _env;
  private readonly ILogger<FileService> _logger;
  private readonly IClaimService _claimService;
  private readonly IAttemptTrackerService _attemptTrackerService;
  private readonly IHttpContextAccessor _httpContextAccessor;
  
  public FileService(
    ApplicationDbContext context,
    IWebHostEnvironment env,
    ILogger<FileService> logger,
    IClaimService claimService,
    IAttemptTrackerService attemptTrackerService,
    IHttpContextAccessor httpContextAccessor
    )
  {
    _context = context;
    _env = env;
    _logger = logger;
    _claimService = claimService;
    _attemptTrackerService = attemptTrackerService;
    _httpContextAccessor = httpContextAccessor;
  }

public async Task<Result<FileUploadResponse>> UploadTempFileAsync(IFormFile file)
{
  if (file == null || file.Length == 0)
    return Result<FileUploadResponse>.Failure(FileErrors.UploadInvalid);

  var ip = _httpContextAccessor.HttpContext?.GetClientIp();
  var key = $"upload:ip:{ip}";

  // 🚫 تحديد معدل المحاولات
  var isLimited = await _attemptTrackerService.IsLimitedAsync(
    key,
    maxAttempts: 5,
    window: TimeSpan.FromMinutes(1),
    lockoutDuration: TimeSpan.FromMinutes(5)
  );

  if (isLimited)
    return Result<FileUploadResponse>.Failure(AuthErrors.TooManyRequests);

  await _attemptTrackerService.RegisterAttemptAsync(key);

  var isAdmin = await _claimService.IsAdminAsync();

  var extension = Path.GetExtension(file.FileName).ToLower();
  var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
  var videoExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm" };

  if (!imageExtensions.Contains(extension) && (!isAdmin || !videoExtensions.Contains(extension)))
    return Result<FileUploadResponse>.Failure(FileErrors.UnsupportedFileType);

  // تحقق من الحجم المسموح
  if (imageExtensions.Contains(extension) && file.Length > 3 * 1024 * 1024) // 3MB
    return Result<FileUploadResponse>.Failure(FileErrors.FileTooLarge);

  if (videoExtensions.Contains(extension) && file.Length > 25 * 1024 * 1024) // 25MB
    return Result<FileUploadResponse>.Failure(FileErrors.FileTooLarge);

  var tempFolder = Path.Combine("uploads", "temp");
  var folderPath = Path.Combine(_env.WebRootPath, tempFolder);
  Directory.CreateDirectory(folderPath);

  var fileName = $"{Guid.NewGuid()}{extension}";
  var filePath = Path.Combine(folderPath, fileName);

  using (var stream = new FileStream(filePath, FileMode.Create))
  {
    await file.CopyToAsync(stream);
  }

  var entity = new MediaFile
  {
    FilePath = Path.Combine("temp", fileName).Replace("\\", "/"),
    IsAttached = false
  };

  _context.MediaFiles.Add(entity);
  await _context.SaveChangesAsync();

  return Result<FileUploadResponse>.Success(new FileUploadResponse
  {
    FileId = entity.Id,
    FilePath = entity.FilePath
  });
}

  public async Task<Result<object>> AttachFileAsync(int fileId, AttachmentEntityType entityType, string entityId)
  {
    var file = await _context.MediaFiles.FindAsync(fileId);
    if (file == null)
      return Result<object>.Failure(FileErrors.FileNotFound);

    if (file.IsAttached)
      return Result<object>.Failure(FileErrors.FileAlreadyAttached);

    var destFolder = Path.Combine("uploads", entityType.ToString().ToLower());
    var destPath = Path.Combine(_env.WebRootPath, destFolder);
    Directory.CreateDirectory(destPath);

    var fileName = Path.GetFileName(file.FilePath);
    var sourcePath = Path.Combine(_env.WebRootPath, "uploads", file.FilePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

    if (!File.Exists(sourcePath))
      return Result<object>.Failure(FileErrors.SourceFileMissing);

    var newFullPath = Path.Combine(destPath, fileName);

    try
    {
      File.Move(sourcePath, newFullPath);
    }
    catch
    {
      return Result<object>.Failure(FileErrors.FileMoveFailed);
    }

    file.FilePath = Path.Combine(destFolder, fileName).Replace("\\", "/");
    file.IsAttached = true;
    file.EntityType = entityType;
    file.EntityId = entityId;

    await _context.SaveChangesAsync();
    return Result<object>.Success(null);
  }

  public async Task<Result<object>> AttachFilesAsync(List<int> fileIds, AttachmentEntityType entityType, string entityId)
  {
    foreach (var id in fileIds)
    {
      var result = await AttachFileAsync(id, entityType, entityId);
      if (!result.IsSuccess)
        return result; // نعيد الخطأ الأصلي دون إنشاء جديد
    }

    return Result<object>.Success(null);
  }

  public async Task<Result<object>> DeleteFilesAsync(List<int> fileIds)
  {
    foreach (var id in fileIds)
    {
      var result = await DeleteFileAsync(id);
      if (!result.IsSuccess)
        return result; // نعيد الخطأ الأصلي دون إنشاء جديد
    }

    return Result<object>.Success(null);
  }


  public async Task<List<FileResponse>> GetFilesByEntityAsync(string entityId, AttachmentEntityType entityType)
  {
    return await _context.MediaFiles
      .Where(f => f.EntityId == entityId && f.EntityType == entityType)
      .Select(f => new FileResponse
        {
          Id = f.Id,
          FilePath = f.FilePath
        }
      )
      .ToListAsync();
  }

  public async Task<FileResponse?> GetFileByEntityAsync(string entityId, AttachmentEntityType entityType)
  {
    return await _context.MediaFiles
      .Where(f => f.EntityId == entityId && f.EntityType == entityType)
      .Select(f => new FileResponse
        {
          Id = f.Id,
          FilePath = f.FilePath
        }
      )
      .SingleOrDefaultAsync();
  }

  public async Task<Result<object>> ProcessFileUpdateAsync(
    int? newFileId,
    int? oldFileId,
    AttachmentEntityType entityType,
    string entityId
  )
  {
    // لا يوجد ملف جديد، ولكن يوجد قديم => احذف
    if (newFileId == null && oldFileId != null)
    {
      var deleteResult = await DeleteFileAsync(oldFileId.Value);
      if (!deleteResult.IsSuccess)
        return Result<object>.Failure(deleteResult.Error);
    }

    // يوجد ملف جديد
    else if (newFileId != null)
    {
      // لو مختلف عن القديم
      if (oldFileId == null || newFileId.Value != oldFileId.Value)
      {
        // جرّب ربط الجديد أولًا
        var attachResult = await AttachFileAsync(newFileId.Value, entityType, entityId);
        if (!attachResult.IsSuccess)
          return Result<object>.Failure(attachResult.Error);

        // بعد نجاح الربط نحذف القديم
        if (oldFileId != null)
        {
          var deleteResult = await DeleteFileAsync(oldFileId.Value);
          if (!deleteResult.IsSuccess)
            return Result<object>.Failure(deleteResult.Error);
        }
      }
    }

    return Result<object>.Success(null);
  }


  public async Task<Result<object>> DeleteFileAsync(int fileId)
  {
    var file = await _context.MediaFiles.FindAsync(fileId);
    if (file == null)
      return Result<object>.Failure(FileErrors.FileNotFound);

    var fullPath = Path.Combine(_env.WebRootPath, file.FilePath ?? "");
    if (File.Exists(fullPath))
    {
      try
      {
        File.Delete(fullPath);
      }
      catch
      {
        return Result<object>.Failure(FileErrors.FileDeleteFailed);
      }
    }

    _context.MediaFiles.Remove(file);
    await _context.SaveChangesAsync();
    return Result<object>.Success(null);
  }

  public async Task<Result<object>> ClearTempFilesAsync()
  {
    try
    {
      Console.WriteLine("[FileService] Clearing temp files");

      var tempFolderPath = Path.Combine(_env.WebRootPath, "uploads", "temp");

      // حذف الملفات الفعلية من المجلد إن وجد
      if (Directory.Exists(tempFolderPath))
      {
        var files = Directory.GetFiles(tempFolderPath);
        foreach (var file in files)
        {
          try
          {
            File.Delete(file);
          }
          catch (Exception ex)
          {
            _logger.LogWarning(ex, "فشل حذف الملف المؤقت: {FilePath}", file);
          }
        }
      }

      // حذف السجلات غير المرفقة من قاعدة البيانات
      var tempRecords = await _context.MediaFiles
        .Where(f => f.IsAttached == false)
        .ToListAsync();

      _context.MediaFiles.RemoveRange(tempRecords);
      await _context.SaveChangesAsync();

      return Result<object>.Success(null);
    }
    catch (Exception)
    {
      return Result<object>.Failure(FileErrors.FileDeleteFailed);
    }
  }


}