using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace TransfersApi.App.__Feature__s.Model
{
  public class __Feature__Configuration : IEntityTypeConfiguration<__Feature__>
  {
    public void Configure(EntityTypeBuilder<__Feature__> builder)
    {
      builder.HasOne(c => c.Country)
        .WithMany()
        .HasForeignKey(c => c.CountryId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}