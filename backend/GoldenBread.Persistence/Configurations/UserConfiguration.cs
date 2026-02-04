using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.UserId).HasName("users_pkey");

        builder.ToTable("users");

        builder.Property(e => e.AccountId).HasColumnName("account_id").IsRequired();
        builder.Property(e => e.Birthday).HasColumnName("birthday");
        builder.Property(e => e.Firstname).HasColumnName("firstname").HasMaxLength(50);
        builder.Property(e => e.Lastname).HasColumnName("lastname").HasMaxLength(50);
        builder.Property(e => e.Patronymic).HasColumnName("patronymic").HasMaxLength(50);

        builder.HasOne(e => e.Account)
            .WithOne(a => a.User)
            .HasForeignKey<User>(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.AccountId).IsUnique();
    }
}