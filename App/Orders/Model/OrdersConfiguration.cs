using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SisApi.App.Orders.Model;

public class OrdersConfiguration : IEntityTypeConfiguration<Order>
{
  public void Configure(EntityTypeBuilder<Order> builder)
  {
    builder.HasKey(order => order.Id);

    builder
      .Property(order => order.Status)
      .IsRequired();

    builder
      .Property(order => order.ClientId)
      .IsRequired();

    builder.ToTable("OrderItems");
    
    builder
      .HasOne(order => order.Client)
      .WithMany()
      .HasForeignKey(order => order.ClientId)
      .OnDelete(DeleteBehavior.Restrict);

    builder
      .HasOne(order => order.Employee)
      .WithMany()
      .HasForeignKey(order => order.EmployeeId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);

    builder
      .HasOne(order => order.Region)
      .WithMany()
      .HasForeignKey(order => order.RegionId)
      .OnDelete(DeleteBehavior.Restrict);

    builder
      .HasOne(order => order.Center)
      .WithMany()
      .HasForeignKey(order => order.CenterId)
      .OnDelete(DeleteBehavior.Restrict);

    builder
      .HasMany(order => order.OrderItems)
      .WithOne(orderItem => orderItem.Order)
      .HasForeignKey(orderItem => orderItem.OrderId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}