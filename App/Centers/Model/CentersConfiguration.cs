using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SisApi.App.Centers.Model;

public class CentersConfiguration :
  IEntityTypeConfiguration<Center>
{
  public void Configure(EntityTypeBuilder<Center> builder)
  {
    builder.HasKey(center => center.Id);

    builder.Property(center => center.Name)
      .IsRequired();

    builder.Property(center => center.Phone)
      .IsRequired();

    builder.Property(center => center.Location)
      .IsRequired();

    // ==========================================
    // المركز الواحد يحتوي على عدة موظفين
    // الموظف يتبع لمركز واحد
    // ==========================================
    builder
      .HasMany(center => center.Employees)
      .WithOne(user => user.Center)
      .HasForeignKey(user => user.CenterId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.Restrict);

    // ==========================================
    // المركز لديه مدير واحد اختياري
    // المدير يدير مركزًا واحدًا كحد أقصى
    // ==========================================
    builder
      .HasOne(center => center.Manager)
      .WithOne(user => user.ManagedCenter)
      .HasForeignKey<Center>(center => center.ManagerId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);
  }
}