using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.EmployeeId).HasName("employees_pkey");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.Property(e => e.Firstname).HasMaxLength(50);
        builder.Property(e => e.Lastname).HasMaxLength(50);
        builder.Property(e => e.Patronymic).HasMaxLength(50);
    }
}
