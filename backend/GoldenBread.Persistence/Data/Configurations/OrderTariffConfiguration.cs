using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class OrderTariffConfiguration : IEntityTypeConfiguration<OrderTariff>
{
    public void Configure(EntityTypeBuilder<OrderTariff> builder)
    {
        builder.HasKey(e => e.OrderTariffId).HasName("order_tariffs_pkey");

        builder.HasQueryFilter(t => t.DeletedAt == null);

        builder.Property(e => e.FreeEmployeesPercent).HasPrecision(4, 2);
        builder.Property(e => e.MarkupPercent).HasPrecision(4, 2);
        builder.Property(e => e.Name).HasMaxLength(100);
    }
}
