using LapisApi.App.MediaFiles.Enums;
namespace LapisApi.App.MediaFiles.Dto;

public class AttachFilesRequest
{
  public List<int> FileIds { get; set; } = null!;
  public AttachmentEntityType EntityType { get; set; }
  public string EntityId { get; set; } = null!;
}