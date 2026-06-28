using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Common.Services;
using GoldenBread.Application.Common.Strategies.Schedule;
using GoldenBread.Application.Features.Orders.Dtos;
using GoldenBread.Domain.Entities;
using System.Diagnostics;

namespace GoldenBread.Application.Features.Orders.Queries;

public sealed record CalculateDeliveryDateQuery(CalculateDeliveryRequest Request) : IRequest<CalculateDeliveryResponse>;

public sealed class CalculateDeliveryDateQueryHandler(
    IGoldenBreadContext context,
    ScheduleTaskDistributor scheduler)
    : IRequestHandler<CalculateDeliveryDateQuery, CalculateDeliveryResponse>
{
    public async Task<CalculateDeliveryResponse> Handle(
        CalculateDeliveryDateQuery query,
        CancellationToken ct)
    {
        var orderItems = new List<OrderItem>();

        foreach (var draft in query.Request.Items)
        {
            var batch = await context.ProductBatches
                .Include(pb => pb.Product)
                .AsNoTracking()
                .FirstAsync(b => b.ProductBatchId == draft.ProductBatchId, ct);

            var orderItem = OrderItem.Create(
                0,
                draft.ProductBatchId,
                draft.Quantity,
                batch.QuantityUnits,
                batch.UnitPrice);
            orderItem.Batch = batch; 

            orderItems.Add(orderItem);
        }

        var employees = await context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeTasks)
            .ToListAsync(ct);

        var jit = new AsapStrategy();
        var scheduleResult = scheduler.Distribute(orderItems, employees, jit);

        DateOnly minimalDate = DateOnly.MinValue;
        DateOnly maximalDate = DateOnly.MaxValue;

        if (scheduleResult.IsFeasible)
        {
            minimalDate = DateOnly.FromDateTime(scheduleResult.PlanEnd);
            maximalDate = DateOnly.FromDateTime(DateTime.Now).AddDays(90);
        }

        return new CalculateDeliveryResponse(minimalDate, maximalDate);
    }
}