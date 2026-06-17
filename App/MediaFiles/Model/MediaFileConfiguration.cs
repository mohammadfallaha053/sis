using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LapisApi.Data.Models;

namespace LapisApi.App.MediaFiles.Model
{
  public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
  {
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
      // builder.HasOne<ApplicationUser>()
      //   .WithMany(u => u.MediaFiles)
      //   .HasForeignKey(f => f.EntityId)
      //   .HasPrincipalKey(u => u.Id)
      //   .OnDelete(DeleteBehavior.SetNull);
    }
  }
}