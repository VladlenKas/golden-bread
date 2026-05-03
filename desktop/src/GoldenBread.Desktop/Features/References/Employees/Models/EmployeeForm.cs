using System.ComponentModel.DataAnnotations;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.Infrastructure.Helpers;
using GoldenBread.Desktop.UI.Common;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.Features.References.Employees.Models;

public partial class EmployeeForm : ViewModelBase
{
    [Reactive]
    int _employeeId;

    [Reactive] 
    [RegularExpression(ConstantRegularExpressions.Name, ErrorMessage = ConstantMessages.NameFormatValidation)]
    [StringLength(35, MinimumLength = 2, ErrorMessage = ConstantMessages.NameLengthValidation)]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    string _firstName = null!;

    [Reactive]
    [RegularExpression(ConstantRegularExpressions.Name, ErrorMessage = ConstantMessages.NameFormatValidation)]
    [StringLength(35, MinimumLength = 2, ErrorMessage = ConstantMessages.NameLengthValidation)]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    string _lastName = null!;

    [Reactive]
    [RegularExpression(ConstantRegularExpressions.NotRequiredName, ErrorMessage = ConstantMessages.NotRequiredNameFormatValidation)]
    string? _patronymic;

    [Reactive] 
    [DateTime]
    DateTimeOffset _birthday;

    public static EmployeeForm FromDto(EmployeeDto dto)
    {
        return new EmployeeForm
        {
            EmployeeId = dto.EmployeeId,
            FirstName = dto.Firstname,
            LastName = dto.Lastname,
            Patronymic = dto.Patronymic,
            Birthday = new DateTimeOffset(dto.Birthday.ToDateTime(TimeOnly.MinValue))
        };
    }

    public EmployeeDto ToDto()
    {
        return new EmployeeDto(
            EmployeeId,
            FirstName,
            LastName,
            Patronymic,
            DateOnly.FromDateTime(Birthday.DateTime));
    }

    public EmployeeForm Clone() => new()
    {
        EmployeeId = EmployeeId,
        FirstName = FirstName,
        LastName = LastName,
        Patronymic = Patronymic,
        Birthday = Birthday
    };

    public bool EqualsValues(EmployeeForm? other)
    {
        if (other is null) return false;
        return EmployeeId == other.EmployeeId
            && FirstName == other.FirstName
            && LastName == other.LastName
            && Patronymic == other.Patronymic
            && Birthday == other.Birthday;
    }
}
