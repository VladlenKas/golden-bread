using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(e => e.CartItemId).HasName("cart_items_new_pkey");

        builder.ToTable("cart_items");

        builder.HasIndex(e => e.BatchId, "fk_cart_items_product_batch_id_idx");
        builder.HasIndex(e => e.AccountId, "fk_cart_items_account_id_idx");

        builder.Property(e => e.CartItemId)
            .HasDefaultValueSql("nextval('cart_items_new_cart_item_id_seq'::regclass)")
            .HasColumnName("cart_item_id");
        builder.Property(e => e.BatchId).HasColumnName("batch_id");
        builder.Property(e => e.Quantity).HasColumnName("quantity");
        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.HasOne(d => d.Batch).WithMany(p => p.CartItems)
            .HasForeignKey(d => d.BatchId)
            .HasConstraintName("fk_cart_items_product_batch_id");

        builder.HasOne(d => d.Account).WithMany(p => p.CartItems)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_cart_items_account_id");
    }
}
