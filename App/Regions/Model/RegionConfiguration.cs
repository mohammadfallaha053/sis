using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace SisApi.App.Regions.Model
{
  public class RegionConfiguration : IEntityTypeConfiguration<Region>
  {
    public void Configure(EntityTypeBuilder<Region> builder)
    {
      builder
        .OwnsMany(
          region => region.Visits,
          visitsBuilder =>
          {
            visitsBuilder.ToJson();
          }
        );
    }
  }
}