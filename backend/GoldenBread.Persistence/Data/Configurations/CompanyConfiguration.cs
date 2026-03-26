using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(e => e.CompanyId).HasName("companies_pkey");

        builder.HasIndex(e => e.AccountId).IsUnique();
        builder.HasIndex(e => e.Name).IsUnique();
        builder.HasIndex(e => e.Inn).IsUnique();
        builder.HasIndex(e => e.Ogrn).IsUnique();

        builder.HasOne(e => e.Account)
            .WithOne(a => a.Company)
            .HasForeignKey<Company>(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Account).AutoInclude();

        builder.Property(e => e.AccountId).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Inn).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Ogrn).HasMaxLength(13).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(11);
    }
}
