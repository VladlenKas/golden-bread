using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(e => e.OrderId)
            .HasName("orders_pkey");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.HasOne(d => d.Company)
            .WithMany(p => p.Orders)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_orders_company_id");
    }
}
