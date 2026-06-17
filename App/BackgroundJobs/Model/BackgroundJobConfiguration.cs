using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LapisApi.App.BackgroundJobs.Model
{
  public class BackgroundJobConfiguration : IEntityTypeConfiguration<BackgroundJob>
  {
    public void Configure(EntityTypeBuilder<BackgroundJob> builder)
    {
    }
  }
}