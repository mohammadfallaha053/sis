using LapisApi.App.MediaFiles.Enums;
using LapisApi.MyEnum;
namespace LapisApi.App.MediaFiles.Model
{
  public class MediaFile
  {
    public int Id { get; set; }

    public string FilePath { get; set; } = null!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public bool IsAttached { get; set; } = false;

    public AttachmentEntityType? EntityType { get; set; }

    public string? EntityId { get; set; }
  }
}