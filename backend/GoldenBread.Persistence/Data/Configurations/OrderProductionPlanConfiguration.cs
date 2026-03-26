//using GoldenBread.Domain.Entities;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace GoldenBread.Infrastructure.Data.Configurations;

//public class OrderProductionPlanConfiguration : IEntityTypeConfiguration<OrderProductionPlan>
//{
//    public void Configure(EntityTypeBuilder<OrderProductionPlan> builder)
//    {
//        builder.HasKey(e => e.PlanId).HasName("order_production_plan_pkey");

//        // Индексы
//        builder.HasIndex(e => e.OrderId, "fk_order_production_plan_order_id_idx");
//        builder.HasIndex(e => e.OrderItemId, "fk_order_production_plan_order_item_id_idx");
//        builder.HasIndex(e => e.EmployeeId, "fk_order_production_plan_employee_id_idx");
//        builder.HasIndex(e => e.PlannedShiftId, "fk_order_production_plan_planned_shift_id_idx");
//        builder.HasIndex(e => new { e.OrderItemId, e.BatchNumber }, "order_production_plan_item_batch_unique").IsUnique();

//        // Фильтрованный индекс для активных задач сотрудника
//        builder.HasIndex(e => e.EmployeeId)
//            .HasFilter("completed_batches < assigned_batches")
//            .HasDatabaseName("idx_order_plan_employee");

//        // Значения по умолчанию
//        builder.Property(e => e.AssignedBatches).HasDefaultValue(1);
//        builder.Property(e => e.CompletedBatches).HasDefaultValue(0);

//        // Relationships
//        builder.HasOne(d => d.Order)
//            .WithMany(p => p.OrderProductionPlans)
//            .HasForeignKey(d => d.OrderId)
//            .OnDelete(DeleteBehavior.Cascade)
//            .HasConstraintName("fk_order_production_plan_order_id");

//        builder.HasOne(d => d.OrderItem)
//            .WithMany(p => p.ProductionPlans)
//            .HasForeignKey(d => d.OrderItemId)
//            .OnDelete(DeleteBehavior.Cascade)
//            .HasConstraintName("fk_order_production_plan_order_item_id");

//        builder.HasOne(d => d.Employee)
//            .WithMany()
//            .HasForeignKey(d => d.EmployeeId)
//            .OnDelete(DeleteBehavior.Restrict)
//            .HasConstraintName("fk_order_production_plan_employee_id");

//        builder.HasOne(d => d.Shift)
//            .WithMany(p => p.OrderProductionPlans)
//            .HasForeignKey(d => d.PlannedShiftId)
//            .OnDelete(DeleteBehavior.Cascade)
//            .HasConstraintName("fk_order_production_plan_planned_shift_id");
//    }
//}
