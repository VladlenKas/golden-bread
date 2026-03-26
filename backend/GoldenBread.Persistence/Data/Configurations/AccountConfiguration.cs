using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(e => e.AccountId).HasName("accounts_pkey");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasIndex(e => e.Email)
            .HasDatabaseName("accounts_email_active_unique")
            .HasFilter("deleted_at IS NULL")
            .IsUnique();

        builder.Navigation(e => e.User).AutoInclude();
        builder.Navigation(e => e.Company).AutoInclude();

        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Session)
            .HasMaxLength(512);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("now()") 
            .ValueGeneratedOnAdd(); ;
    }
}
