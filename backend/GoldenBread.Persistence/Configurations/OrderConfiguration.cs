using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(e => e.OrderId).HasName("orders_pkey");

        builder.ToTable("orders");

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        builder.Property(e => e.EndDate).HasColumnName("end_date");
        builder.Property(e => e.StartDate).HasColumnName("start_date");
        builder.Property(e => e.TariffId).HasColumnName("tariff_id");
        builder.Property(e => e.CompanyId).HasColumnName("company_id");

        builder.HasOne(d => d.Tariff).WithMany(p => p.Orders)
            .HasForeignKey(d => d.TariffId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_orders_tariff_id");

        builder.HasOne(d => d.Company).WithMany(p => p.Orders)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_orders_company_id");
    }
}
