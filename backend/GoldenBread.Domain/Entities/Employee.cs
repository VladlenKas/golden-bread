namespace GoldenBread.Domain.Entities;

public class Employee
{
    public int EmployeeId { get; set; }

    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateOnly Birthday { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<EmployeeTask> EmployeeTasks { get; set; } = new List<EmployeeTask>();
}
