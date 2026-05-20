using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class OrderItemIngredientReservationConfiguration : IEntityTypeConfiguration<OrderItemIngredientReservation>
{
    public void Configure(EntityTypeBuilder<OrderItemIngredientReservation> builder)
    {
        builder.HasKey(e => e.OrderItemIngredientReservationId)
            .HasName("order_item_ingredient_reservations_pkey");

        builder.Property(e => e.ReservedQuantity)
            .HasPrecision(18, 4);

        builder.HasOne(d => d.OrderItem)
            .WithMany(p => p.IngredientReservations)
            .HasForeignKey(d => d.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_reservations_order_item_id");

        builder.HasOne(d => d.IngredientBatch)
            .WithMany(p => p.OrderItemReservations)
            .HasForeignKey(d => d.IngredientBatchId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_reservations_ingredient_batch_id");

        builder.HasIndex(e => e.OrderItemId)
            .HasDatabaseName("idx_reservations_order_item");

        builder.HasIndex(e => e.IngredientBatchId)
            .HasDatabaseName("idx_reservations_ingredient_batch");
    }
}
