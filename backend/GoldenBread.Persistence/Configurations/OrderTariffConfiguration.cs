using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class OrderTariffConfiguration : IEntityTypeConfiguration<OrderTariff>
{
    public void Configure(EntityTypeBuilder<OrderTariff> builder)
    {
        builder.HasKey(e => e.OrderTariffId).HasName("order_tariffs_pkey");

        builder.ToTable("order_tariffs");

        builder.Property(e => e.OrderTariffId).HasColumnName("order_tariff_id");
        builder.Property(e => e.IsDelete)
            .HasDefaultValue((short)0)
            .HasColumnName("is_delete");
        builder.Property(e => e.Description).HasColumnName("description");
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
