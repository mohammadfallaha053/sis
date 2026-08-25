using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SisApi.App.Products.Model;

public class ProductsConfiguration : IEntityTypeConfiguration<Product>
{
  public void Configure(EntityTypeBuilder<Product> builder)
  {
    builder.ToTable("Products");

    builder.HasKey(product => product.Id);

    builder
      .Property(product => product.Name)
      .IsRequired()
      .HasMaxLength(200);

    builder
      .Property(product => product.Description)
      .HasMaxLength(1000);

    builder
      .Property(product => product.PointsPrice)
      .HasPrecision(18, 2);

    builder
      .HasOne(product => product.Category)
      .WithMany(category => category.Products)
      .HasForeignKey(product => product.CategoryId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
