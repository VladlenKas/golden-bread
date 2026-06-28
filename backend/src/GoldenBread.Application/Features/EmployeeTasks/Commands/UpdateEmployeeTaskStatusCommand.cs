using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.EmployeeTasks.Dtos;
using GoldenBread.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Application.Features.EmployeeTasks.Commands;

public sealed record UpdateEmployeeTaskStatusCommand(
    UpdateEmployeeTaskStatusRequest Request) : IRequest<bool>;

public sealed class UpdateEmployeeTaskStatusCommandHandler(
    IGoldenBreadContext context,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeTaskStatusCommand, bool>
{
    public async Task<bool> Handle(
        UpdateEmployeeTaskStatusCommand command,
        CancellationToken ct)
    {
        var req = command.Request;
        var task = await context.EmployeeTasks
            .AsTracking()
            .FirstOrDefaultAsync(t => t.EmployeeTaskId == req.EmployeeTaskId, ct);

        if (task is null)
            return false;

        // Бизнес‑правила
        if (!(task.StartTime.HasValue && task.StartTime.Value.Date >= DateTimeOffset.Now.Date) &&
            task.Status == Domain.Enums.TaskStatus.Completed)
            throw new InvalidOperationException("Прошлые выполненные задачи нельзя изменять");

        if (task.Status == Domain.Enums.TaskStatus.Canceled || req.NewStatus == Domain.Enums.TaskStatus.Canceled)
            throw new InvalidOperationException("Отмененные задачи нельзя изменять");

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            task.UpdateStatus(req.NewStatus);
            await unitOfWork.CommitAsync(ct);
            return true;
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}