using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LapisApi.Data.Models;

namespace LapisApi.App.Cities.Model
{
  public class CityConfiguration : IEntityTypeConfiguration<City>
  {
    public void Configure(EntityTypeBuilder<City> builder)
    {
      builder.Property(c => c.MaximumTransferLimit).HasPrecision(18, 2);
    }
  }
}