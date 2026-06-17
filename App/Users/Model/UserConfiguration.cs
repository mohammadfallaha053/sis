// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using LapisApi.Models;
// namespace LapisApi.App.Users.Model
// {
//   public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
//   {
//     public void Configure(EntityTypeBuilder<ApplicationUser> builder)
//     {
//       builder.HasOne(c => c.Center)
//         .WithMany(cn => cn.Agents)
//         .HasForeignKey(c => c.CenterId)
//         .IsRequired(false)
//         .OnDelete(DeleteBehavior.Restrict);
//     }
//   }
// }