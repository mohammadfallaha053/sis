using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace SisApi.App.ItemTypes.Model
{
  public class ItemTypesConfiguration : IEntityTypeConfiguration<ItemType>
  {
    public void Configure(EntityTypeBuilder<ItemType> builder)
    {

    }
  }
}