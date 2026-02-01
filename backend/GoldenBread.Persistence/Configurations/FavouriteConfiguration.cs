using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class FavouriteConfiguration : IEntityTypeConfiguration<Favourite>
{
    public void Configure(EntityTypeBuilder<Favourite> builder)
    {
        builder.HasKey(e => e.FavouriteId).HasName("favourites_pkey");

        builder.ToTable("favourites");

        builder.HasIndex(e => e.ProductId, "fk_favourites_product_id_idx");

        builder.HasIndex(e => e.AccountId, "fk_favourites_account_id_idx");

        builder.Property(e => e.FavouriteId).HasColumnName("favourite_id");
        builder.Property(e => e.ProductId).HasColumnName("product_id");
        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.HasOne(d => d.Product).WithMany(p => p.Favourites)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_favourites_product_id");

        builder.HasOne(d => d.Account).WithMany(p => p.Favourites)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_favourites_account_id");
    }
}
