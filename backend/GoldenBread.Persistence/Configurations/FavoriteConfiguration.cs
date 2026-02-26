using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasKey(e => e.FavoriteId).HasName("favorites_pkey");
        builder.ToTable("favorites");

        builder.Property(e => e.ProductId)
            .HasColumnName("product_id");

        builder.Property(e => e.CompanyId)
            .HasColumnName("company_id");

        builder.HasOne(d => d.Product)
            .WithMany(p => p.Favourites)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_favorites_product_id");

        builder.HasOne(d => d.Company)
            .WithMany(p => p.Favourites)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_favorites_company_id");
    }
}
