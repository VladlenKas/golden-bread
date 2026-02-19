using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(e => e.CartItemId).HasName("cart_items_new_pkey");

        builder.ToTable("cart_items");

        builder.Property(e => e.BatchId).HasColumnName("batch_id");
        builder.Property(e => e.Quantity).HasColumnName("quantity");
        builder.Property(e => e.CompanyId).HasColumnName("company_id");

        builder.HasOne(d => d.Batch).WithMany(p => p.CartItems)
            .HasForeignKey(d => d.BatchId)
            .HasConstraintName("fk_cart_items_product_batch_id");

        builder.HasOne(d => d.Company).WithMany(p => p.CartItems)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_cart_items_company_id");
    }
}
