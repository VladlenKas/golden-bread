using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class EmployeeTaskConfiguration : IEntityTypeConfiguration<EmployeeTask>
{
    public void Configure(EntityTypeBuilder<EmployeeTask> builder)
    {
        builder.HasKey(e => e.EmployeeTaskId).HasName("employee_tasks_new_pkey");

        builder.HasIndex(e => e.EmployeeId, "fk_employee_tasks_employee_id_idx");
        builder.HasIndex(e => e.OrderItemId, "fk_employee_tasks_order_item_id_idx");

        builder.HasOne(d => d.Employee)
            .WithMany(p => p.EmployeeTasks)
            .HasForeignKey(d => d.EmployeeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_employee_tasks_employee_id");

        builder.HasOne(d => d.OrderItem)
            .WithMany(p => p.EmployeeTasks)
            .HasForeignKey(d => d.OrderItemId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_employee_tasks_order_item_id");
    }
}
