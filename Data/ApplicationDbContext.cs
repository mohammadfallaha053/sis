using LapisApi.App.MediaFiles.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SisApi.App.Centers.Model;
using SisApi.App.MediaFiles.Model;
using SisApi.App.Regions.Model;
using SisApi.App.Settings.Model;
using SisApi.App.Users.Model;
namespace SisApi.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Region> Regions { get; set; }
    public DbSet<Center> Centers { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
  }
}