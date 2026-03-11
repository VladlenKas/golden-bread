using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(e => e.CartItemId).HasName("cart_items_new_pkey");

        builder.HasOne(d => d.Batch)
            .WithMany(p => p.CartItems)
            .HasForeignKey(d => d.BatchId)
            .HasConstraintName("fk_cart_items_product_batch_id");

        builder.HasOne(d => d.Company)
            .WithMany(p => p.CartItems)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_cart_items_company_id");
    }
}
