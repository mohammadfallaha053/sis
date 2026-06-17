using LapisApi.App.MediaFiles.Dto;
using LapisApi.App.MediaFiles.Enums;
using LapisApi.MyEnum;
namespace LapisApi.App.MediaFiles.Interfaces;

public interface IFileService
{

  Task<Result<FileUploadResponse>> UploadTempFileAsync(IFormFile file);
  Task<Result<object>> AttachFileAsync(int fileId, AttachmentEntityType entityType, string entityId);
  Task<Result<object>> AttachFilesAsync(List<int> fileIds, AttachmentEntityType entityType, string entityId);

  Task<Result<object>> ClearTempFilesAsync();
  Task<List<FileResponse>> GetFilesByEntityAsync(
    string entityId,
    AttachmentEntityType entityType
  );

  Task<FileResponse?> GetFileByEntityAsync(
    string entityId,
    AttachmentEntityType entityType
  );

  Task<Result<object>> ProcessFileUpdateAsync(
    int? newFileId,
    int? oldFileId,
    AttachmentEntityType entityType,
    string entityId
  );

  Task<Result<object>> DeleteFileAsync(int fileId);

  Task<Result<object>> DeleteFilesAsync(List<int> fileIds);


}