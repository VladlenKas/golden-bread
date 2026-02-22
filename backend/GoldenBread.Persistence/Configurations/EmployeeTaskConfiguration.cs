using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class EmployeeTaskConfiguration : IEntityTypeConfiguration<EmployeeTask>
{
    public void Configure(EntityTypeBuilder<EmployeeTask> builder)
    {
        builder.HasKey(e => e.EmployeeTaskId).HasName("employee_tasks_new_pkey");
        builder.ToTable("employee_tasks");

        builder.HasIndex(e => e.EmployeeId, "fk_employee_tasks_employee_id_idx");
        builder.HasIndex(e => e.OrderItemId, "fk_employee_tasks_order_item_id_idx");

        builder.Property(e => e.EmployeeTaskId)
            .HasColumnName("employee_task_id");

        builder.Property(e => e.AssignedQuantity)
            .HasColumnName("assigned_quantity");

        builder.Property(e => e.CompletedQuantity)
            .HasDefaultValue(0)
            .HasColumnName("completed_quantity");

        builder.Property(e => e.EmployeeId)
            .HasColumnName("employee_id");

        builder.Property(e => e.EndTime)
            .HasColumnName("end_time");

        builder.Property(e => e.OrderItemId)
            .HasColumnName("order_item_id");

        builder.Property(e => e.StartTime)
            .HasColumnName("start_time");

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
