using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LapisApi.App.Centers.Model
{
  public class CenterConfiguration : IEntityTypeConfiguration<Center>
  {
    public void Configure(EntityTypeBuilder<Center> builder)
    {
      builder.HasOne(c => c.Region)
        .WithMany(cn => cn.Centers)
        .HasForeignKey(c => c.RegionId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}
