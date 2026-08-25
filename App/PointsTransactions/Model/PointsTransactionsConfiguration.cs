using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SisApi.App.PointsTransactions.Model;

public class PointsTransactionsConfiguration :
  IEntityTypeConfiguration<PointsTransaction>
{
  public void Configure(EntityTypeBuilder<PointsTransaction> builder)
  {
    builder.ToTable("PointsTransactions");

    builder.HasKey(transaction => transaction.Id);

    builder
      .Property(transaction => transaction.ClientId)
      .IsRequired();

    builder
      .Property(transaction => transaction.Points)
      .HasPrecision(18, 2);

    builder
      .Property(transaction => transaction.BalanceBefore)
      .HasPrecision(18, 2);

    builder
      .Property(transaction => transaction.BalanceAfter)
      .HasPrecision(18, 2);

    builder
      .HasOne(transaction => transaction.Client)
      .WithMany()
      .HasForeignKey(transaction => transaction.ClientId)
      .OnDelete(DeleteBehavior.Restrict);

    builder
      .HasOne(transaction => transaction.Order)
      .WithMany()
      .HasForeignKey(transaction => transaction.OrderId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.Restrict);

    builder
      .HasOne(transaction => transaction.Product)
      .WithMany()
      .HasForeignKey(transaction => transaction.ProductId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.Restrict);

    // Each completed order can credit points only once.
    builder
      .HasIndex(transaction => transaction.OrderId)
      .IsUnique()
      .HasFilter("[OrderId] IS NOT NULL");
  }
}
