using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SisApi.App.Categories.Model;

public class CategoriesConfiguration : IEntityTypeConfiguration<Category>
{
  public void Configure(EntityTypeBuilder<Category> builder)
  {
    builder.ToTable("Categories");

    builder.HasKey(category => category.Id);

    builder
      .Property(category => category.Name)
      .IsRequired()
      .HasMaxLength(200);
  }
}
