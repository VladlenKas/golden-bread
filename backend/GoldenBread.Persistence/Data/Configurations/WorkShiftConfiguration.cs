//using GoldenBread.Domain.Entities;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace GoldenBread.Infrastructure.Data.Configurations;

//public class WorkShiftConfiguration : IEntityTypeConfiguration<WorkShift>
//{
//    public void Configure(EntityTypeBuilder<WorkShift> builder)
//    {
//        builder.HasKey(e => e.ShiftId).HasName("work_shifts_pkey");

//        builder.HasIndex(e => e.EmployeeId, "fk_work_shifts_employee_id_idx");
//        builder.HasIndex(e => e.ShiftDate, "fk_work_shifts_shift_date_idx");
//        builder.HasIndex(e => new { e.EmployeeId, e.ShiftDate }, "work_shifts_employee_shift_unique").IsUnique();


//        builder.HasOne(d => d.Employee)
//            .WithMany(p => p.WorkShifts)
//            .HasForeignKey(d => d.EmployeeId)
//            .OnDelete(DeleteBehavior.Cascade)
//            .HasConstraintName("fk_work_shifts_employee_id");

//        builder.HasMany(d => d.OrderProductionPlans)
//            .WithOne(p => p.Shift)
//            .HasForeignKey(d => d.PlannedShiftId)
//            .OnDelete(DeleteBehavior.Cascade)
//            .HasConstraintName("fk_order_production_plan_planned_shift_id");
//    }
//}
