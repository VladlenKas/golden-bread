using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(e => e.OrderItemId).HasName("order_items_new_pkey");
        builder.ToTable("order_items");

        builder.HasIndex(e => e.OrderId, "fk_order_items_order_id_idx");
        builder.HasIndex(e => e.BatchId, "fk_order_items_product_batch_id_idx");

        builder.Property(e => e.OrderItemId)
            .HasDefaultValueSql("nextval('order_items_new_order_item_id_seq'::regclass)")
            .HasColumnName("order_item_id");

        builder.Property(e => e.BatchId)
            .HasColumnName("batch_id");

        builder.Property(e => e.OrderId)
            .HasColumnName("order_id");

        builder.Property(e => e.Quantity)
            .HasColumnName("quantity");

        builder.Property(e => e.UnitPriceAtOrder)
            .HasColumnName("unit_price_at_order ");

        builder.Property(e => e.TotalUnits)
            .HasColumnName("total_units");

        builder.HasOne(d => d.Batch)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(d => d.BatchId)
            .HasConstraintName("fk_order_items_product_batch_id");

        builder.HasOne(d => d.Order)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_order_items_order_id");
    }
}
