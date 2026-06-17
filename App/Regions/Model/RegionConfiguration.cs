
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LapisApi.Data.Models;
namespace LapisApi.App.Regions.Model
{
  public class RegionConfiguration : IEntityTypeConfiguration<Region>
  {
    public void Configure(EntityTypeBuilder<Region> builder)
    {
      builder.HasOne(c => c.City)
        .WithMany(cn => cn.Regions)
        .HasForeignKey(c => c.CityId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}