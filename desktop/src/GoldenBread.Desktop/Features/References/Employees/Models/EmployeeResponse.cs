using System.ComponentModel.DataAnnotations;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.Infrastructure.Helpers;
using GoldenBread.Desktop.UI.Common;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.Features.References.Employees.Models;

public partial class EmployeeResponse : ViewModelBase
{
    [Reactive]
    int _employeeId;

    [Reactive] [Required(ErrorMessage = ConstantMessages.RequiredField)]
    string? _firstName;

    [Reactive] [Required(ErrorMessage = ConstantMessages.RequiredField)]
    string? _lastName;

    [Reactive]
    string? _patronymic;

    [Reactive] [DateTime]
    DateTimeOffset _birthday;

    public EmployeeResponse Clone() => new()
    {
        EmployeeId = EmployeeId,
        FirstName = FirstName,
        LastName = LastName,
        Patronymic = Patronymic,
        Birthday = Birthday
    };

    public bool EqualsValues(EmployeeResponse? other)
    {
        if (other is null) return false;
        return EmployeeId == other.EmployeeId
            && FirstName == other.FirstName
            && LastName == other.LastName
            && Patronymic == other.Patronymic
            && Birthday == other.Birthday;
    }
}
