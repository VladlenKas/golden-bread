using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(e => e.SupplierId).HasName("suppliers_pkey");
        builder.ToTable("suppliers");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.Property(e => e.SupplierId)
            .HasColumnName("supplier_id");

        builder.Property(e => e.Address)
            .HasColumnName("address");

        builder.Property(e => e.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .HasColumnName("email");

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(e => e.Phone)
            .HasMaxLength(11)
            .HasColumnName("phone");
    }
}
