using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class OrderTariffConfiguration : IEntityTypeConfiguration<OrderTariff>
{
    public void Configure(EntityTypeBuilder<OrderTariff> builder)
    {
        builder.HasKey(e => e.OrderTariffId).HasName("order_tariffs_pkey");
        builder.ToTable("order_tariffs");

        builder.HasQueryFilter(t => t.DeletedAt == null);

        builder.Property(e => e.OrderTariffId)
            .HasColumnName("order_tariff_id");

        builder.Property(e => e.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(e => e.Description)
            .HasColumnName("description");

        builder.Property(e => e.FreeEmployeesPercent)
            .HasPrecision(4, 2)
            .HasColumnName("free_employees_percent");

        builder.Property(e => e.MarkupPercent)
            .HasPrecision(4, 2)
            .HasColumnName("markup_percent");

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .HasColumnName("name");
    }
}
