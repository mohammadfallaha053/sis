using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SisApi.App.Orders.Model
{
  public class OrderItemConfiguration :
    IEntityTypeConfiguration<OrderItem>
  {
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
      builder.HasKey(orderItem => orderItem.Id);

      builder
        .Property(orderItem => orderItem.WeightKg)
        .HasPrecision(18, 3);

      builder
        .Property(orderItem => orderItem.Points)
        .HasPrecision(18, 2);

      builder
        .Property(orderItem => orderItem.PointsPerKg)
        .IsRequired();

      // ==========================================
      // العنصر تابع لطلب واحد
      // ==========================================
      
      builder.ToTable("Orders");
      
      builder
        .HasOne(orderItem => orderItem.Order)
        .WithMany(order => order.OrderItems)
        .HasForeignKey(orderItem => orderItem.OrderId)
        .OnDelete(DeleteBehavior.Cascade);

      // ==========================================
      // العنصر مرتبط بنوع مادة واحد
      // ==========================================
      builder
        .HasOne(orderItem => orderItem.ItemType)
        .WithMany()
        .HasForeignKey(orderItem => orderItem.ItemTypeId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}