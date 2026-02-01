using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.EmployeeId).HasName("employees_pkey");

        builder.ToTable("employees");

        builder.Property(e => e.EmployeeId).HasColumnName("employee_id");
        builder.Property(e => e.Birthday).HasColumnName("birthday");
        builder.Property(e => e.IsDelete)
            .HasDefaultValue((short)0)
            .HasColumnName("is_delete");
        builder.Property(e => e.Firstname)
            .HasMaxLength(50)
            .HasColumnName("firstname");
        builder.Property(e => e.Lastname)
            .HasMaxLength(50)
            .HasColumnName("lastname");
        builder.Property(e => e.Patronymic)
            .HasMaxLength(50)
            .HasColumnName("patronymic");
    }
}