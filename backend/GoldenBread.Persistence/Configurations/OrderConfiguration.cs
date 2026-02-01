using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(e => e.OrderId).HasName("orders_pkey");

        builder.ToTable("orders");

        builder.HasIndex(e => e.TariffId, "fk_orders_tariff_id_idx");

        builder.HasIndex(e => e.AccountId, "fk_orders_account_id_idx");

        builder.Property(e => e.OrderId).HasColumnName("order_id");
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        builder.Property(e => e.EndDate).HasColumnName("end_date");
        builder.Property(e => e.StartDate).HasColumnName("start_date");
        builder.Property(e => e.TariffId).HasColumnName("tariff_id");
        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.HasOne(d => d.Tariff).WithMany(p => p.Orders)
            .HasForeignKey(d => d.TariffId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_orders_tariff_id");

        builder.HasOne(d => d.Account).WithMany(p => p.Orders)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_orders_account_id");
    }
}
