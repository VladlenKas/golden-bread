using GoldenBread.Domain.Entities;

namespace GoldenBread.Domain.Interfaces.Services;

public interface IEmployeeTaskDistributor
{
    IReadOnlyList<EmployeeTaskAssignment> Distribute(
        OrderItem orderItem,
        IReadOnlyList<Employee> employees,
        Dictionary<int, DateTime> employeeAvailableFrom,
        decimal freeEmployeesPercent,
        DateTime currentTime);
}

public record EmployeeTaskAssignment(
    int EmployeeId,
    int OrderItemId,
    DateTime StartTime,
    DateTime EndTime,
    int AssignedQuantity);

