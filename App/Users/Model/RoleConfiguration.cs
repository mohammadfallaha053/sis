using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LapisApi.App.Users.Model
{
  public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
  {
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
      builder.HasData(
        new IdentityRole
        {
          Id = "1",
          Name = "Admin",
          NormalizedName = "ADMIN"
        },
        new IdentityRole
        {
          Id = "2",
          Name = "Agent",
          NormalizedName = "AGENT"
        },
        new IdentityRole
        {
          Id = "3",
          Name = "Client",
          NormalizedName = "SENDER"
        }
      );
    }
  }
}