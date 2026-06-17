using LapisApi.App.MediaFiles.Enums;
using LapisApi.App.MediaFiles.Interfaces;
namespace LapisApi.Helpers;

public static class FileMergerHelper
{
  public static async Task AddEntityImagesAsync<TDto, TEntity>(
    List<TDto> dtos,
    List<TEntity> entities,
    Func<TEntity, string> getEntityId,
    Action<TDto, string?> setImageUrl,
    IFileService fileService,
    AttachmentEntityType entityType)
  {
    if (dtos.Count != entities.Count)
      throw new ArgumentException("DTOs and Entities count must match");

    for (int i = 0; i < dtos.Count; i++)
    {
      var entityId = getEntityId(entities[i]);
      var file = await fileService.GetFileByEntityAsync(entityId, entityType);
      setImageUrl(dtos[i], file?.FilePath);
    }
  }
}