using LapisApi.App.MediaFiles.Dto;
using LapisApi.App.MediaFiles.Enums;
namespace SisApi.App.MediaFiles.Interfaces;

public interface IFileService
{

  Task<Result<FileUploadResponse>> UploadTempFileAsync(IFormFile file);
  Task<Result<FileResponse>> AttachFileAsync(int fileId, AttachmentEntityType entityType, string entityId);
  Task<Result<List<FileResponse>>> AttachFilesAsync(List<int> fileIds, AttachmentEntityType entityType, string entityId);

  Task<Result<object>> ClearTempFilesAsync();
  Task<List<FileResponse>> GetFilesByEntityAsync(
    string entityId,
    AttachmentEntityType entityType
  );

  Task<FileResponse?> GetFileByEntityAsync(
    string entityId,
    AttachmentEntityType entityType
  );

  Task<Result<FileResponse?>> ProcessFileUpdateAsync(
    int? newFileId,
    int? oldFileId,
    AttachmentEntityType entityType,
    string entityId
  );

  Task<Result<FileResponse?>> ProcessSingleFileUpdateAsync(
    int? newFileId,
    AttachmentEntityType entityType,
    string entityId
  );

  Task<Result<object>> DeleteFileAsync(int fileId);

  Task<Result<object>> DeleteFilesAsync(List<int> fileIds);

  Task<Dictionary<string, FileResponse>> GetFirstFilesByEntitiesAsync(
    List<string> entityIds,
    AttachmentEntityType entityType
  );

}