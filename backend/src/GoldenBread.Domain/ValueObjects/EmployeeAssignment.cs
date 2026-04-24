using GoldenBread.Domain.Entities;

namespace GoldenBread.Domain.ValueObjects;

public sealed record EmployeeAssignment(
    Employee Employee,
    int AssignedQuantityUnits);

