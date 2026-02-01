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

        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.Birthday).HasColumnName("birthday");
        builder.Property(e => e.Firstname)
            .HasMaxLength(50)
            .HasColumnName("firstname");
        builder.Property(e => e.Lastname)
            .HasMaxLength(50)
            .HasColumnName("lastname");
        builder.Property(e => e.Patronymic)
            .HasMaxLength(50)
            .HasColumnName("patronymic");
        builder.Property(e => e.AccountId).HasColumnName("account_id")
            .IsRequired();

        builder.HasOne(e => e.Account)
            .WithOne(a => a.User)
            .HasForeignKey<User>(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.AccountId).IsUnique();
    }
}