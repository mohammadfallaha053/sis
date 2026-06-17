using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LapisApi.App.Centers.Model;
using LapisApi.App.Cities.Model;
using LapisApi.App.MediaFiles.Model;
using LapisApi.App.Settings.Model;
using LapisApi.App.Users.Model;
using LapisApi.Data.Models;

namespace LapisApi.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Region> Regions { get; set; }
    public DbSet<City> Cities { get; set; }
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