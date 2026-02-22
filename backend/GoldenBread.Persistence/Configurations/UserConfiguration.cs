using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(e => e.UserId).HasName("users_pkey");
        entity.ToTable("users");

        entity.HasIndex(e => e.AccountId).IsUnique();

        entity.Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        entity.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        entity.Property(e => e.Birthday)
            .HasColumnName("birthday");

        entity.Property(e => e.Firstname)
            .HasColumnName("firstname")
            .HasMaxLength(50);

        entity.Property(e => e.Lastname)
            .HasColumnName("lastname")
            .HasMaxLength(50);

        entity.Property(e => e.Patronymic)
            .HasColumnName("patronymic")
            .HasMaxLength(50);

        entity.HasOne(e => e.Account)
            .WithOne(a => a.User)
            .HasForeignKey<User>(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
