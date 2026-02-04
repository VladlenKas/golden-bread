using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> entity)
    {
        entity.HasKey(e => e.AccountId).HasName("accounts_pkey");

        entity.ToTable("accounts");
        
        entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        entity.Property(e => e.AccountType).HasColumnName("account_type").IsRequired();
        entity.Property(e => e.VerificationStatus).HasColumnName("verification_status").IsRequired();
        entity.Property(e => e.Session).HasColumnName("session").HasMaxLength(512);
        entity.Property(e => e.SessionExpiresAt).HasColumnName("session_expires_at");
        entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue((short)0);

        entity.HasIndex(e => e.Email)
            .HasDatabaseName("accounts_email_active_unique")
            .HasFilter("is_active != 0")
            .IsUnique();
            
        entity.HasQueryFilter(e => e.IsActive != 0);
        
        entity.Navigation(e => e.User).AutoInclude();
        entity.Navigation(e => e.Company).AutoInclude();
    }
}