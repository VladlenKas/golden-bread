using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.UserId).HasName("users_pkey");

        builder.HasIndex(e => e.AccountId).IsUnique();

        builder.HasOne(e => e.Account)
            .WithOne(a => a.User)
            .HasForeignKey<User>(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.AccountId).IsRequired();
        builder.Property(e => e.Firstname).HasMaxLength(50);
        builder.Property(e => e.Lastname).HasMaxLength(50);
        builder.Property(e => e.Patronymic).HasMaxLength(50);
    }
}
