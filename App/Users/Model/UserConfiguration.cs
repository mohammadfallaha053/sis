using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace SisApi.App.Users.Model
{
  public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
  {
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
      builder
        .Property(user => user.PointsBalance)
        .HasPrecision(18, 2);
    }
  }
}