using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(e => e.SupplierId).HasName("suppliers_pkey");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.Property(e => e.Email).HasMaxLength(255);
        builder.Property(e => e.Name).HasMaxLength(200);
        builder.Property(e => e.Phone).HasMaxLength(11);
    }
}
