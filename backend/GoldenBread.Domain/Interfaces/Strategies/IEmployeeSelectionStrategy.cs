using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Domain.Interfaces.Strategies;

public interface IEmployeeSelectionStrategy
{
    string Name { get; }

    List<EmployeeAssignment> Select(
        OrderItem orderItem,
        List<Employee> candidates,
        SchedulingContext context);  
}
