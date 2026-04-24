namespace GoldenBread.Domain.Entities;

public class Employee
{
    public int EmployeeId { get; private set; }

    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateOnly Birthday { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<EmployeeTask> EmployeeTasks { get; set; } = new List<EmployeeTask>();

    public Employee() { }

    public static Employee Create(
        int employeeId,
        string firstname,
        string lastname,
        string? patronymic,
        DateOnly birthday,
        ICollection<EmployeeTask>? employeeTasks = null)
    {
        return new Employee
        {
            EmployeeId = employeeId,
            Firstname = firstname,
            Lastname = lastname,
            Patronymic = patronymic,
            Birthday = birthday,
            EmployeeTasks = employeeTasks ?? new List<EmployeeTask>()
        };
    }
}
