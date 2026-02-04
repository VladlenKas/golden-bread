using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(e => e.CompanyId).HasName("companies_pkey");

        builder.ToTable("companies");
        
        builder.Property(e => e.CompanyId).HasColumnName("company_id");
        builder.Property(e => e.AccountId).HasColumnName("account_id").IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(e => e.Inn).HasColumnName("inn").HasMaxLength(12).IsRequired();
        builder.Property(e => e.Ogrn).HasColumnName("ogrn").HasMaxLength(13).IsRequired();
        builder.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(11);
        builder.Property(e => e.Address).HasColumnName("address");

        builder.HasOne(e => e.Account)
            .WithOne(a => a.Company)
            .HasForeignKey<Company>(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.AccountId).IsUnique();
        builder.HasIndex(e => e.Name).IsUnique();
        builder.HasIndex(e => e.Inn).IsUnique();
        builder.HasIndex(e => e.Ogrn).IsUnique();
    }
}