using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class FavouriteConfiguration : IEntityTypeConfiguration<Favourite>
{
    public void Configure(EntityTypeBuilder<Favourite> builder)
    {
        builder.HasKey(e => e.FavouriteId).HasName("favourites_pkey");

        builder.ToTable("favourites");

        builder.Property(e => e.ProductId).HasColumnName("product_id");
        builder.Property(e => e.CompanyId).HasColumnName("company_id");

        builder.HasOne(d => d.Product).WithMany(p => p.Favourites)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_favourites_product_id");

        builder.HasOne(d => d.Company).WithMany(p => p.Favourites)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_favourites_company_id");
    }
}
