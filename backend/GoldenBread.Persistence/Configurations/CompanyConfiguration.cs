using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> entity)
    {
        entity.HasKey(e => e.CompanyId).HasName("companies_pkey");
        entity.ToTable("companies");

        entity.HasIndex(e => e.AccountId).IsUnique();
        entity.HasIndex(e => e.Name).IsUnique();
        entity.HasIndex(e => e.Inn).IsUnique();
        entity.HasIndex(e => e.Ogrn).IsUnique();

        entity.HasOne(e => e.Account)
            .WithOne(a => a.Company)
            .HasForeignKey<Company>(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        entity.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        entity.Property(e => e.Inn)
            .HasColumnName("inn")
            .HasMaxLength(10)
            .IsRequired();

        entity.Property(e => e.Ogrn)
            .HasColumnName("ogrn")
            .HasMaxLength(13)
            .IsRequired();

        entity.Property(e => e.Phone)
            .HasColumnName("phone")
            .HasMaxLength(11);

        entity.Property(e => e.Address)
            .HasColumnName("address");
    }
}
