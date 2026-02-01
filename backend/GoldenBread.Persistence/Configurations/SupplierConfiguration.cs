using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(e => e.SupplierId).HasName("suppliers_pkey");

        builder.ToTable("suppliers");

        builder.Property(e => e.SupplierId).HasColumnName("supplier_id");
        builder.Property(e => e.Address).HasColumnName("address");
        builder.Property(e => e.IsDelete)
            .HasDefaultValue((short)0)
            .HasColumnName("is_delete");
        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .HasColumnName("email");
        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .HasColumnName("name");
        builder.Property(e => e.Phone)
            .HasMaxLength(11)
            .HasColumnName("phone");

        builder.HasQueryFilter(e => e.IsDelete == 0);
    }
}
