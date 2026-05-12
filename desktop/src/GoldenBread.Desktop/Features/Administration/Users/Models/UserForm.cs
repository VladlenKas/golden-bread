using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.Infrastructure.Helpers;
using GoldenBread.Desktop.UI.Common;
using System.ComponentModel.DataAnnotations;
using ReactiveUI.SourceGenerators;
using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.Features.Common.Account;
using GoldenBread.Desktop.Features.Common;

namespace GoldenBread.Desktop.Features.Administration.SystemUsers.Models;

public partial class UserForm : ViewModelBase
{
    [Reactive]
    int _userId;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(35, MinimumLength = 2, ErrorMessage = ConstantMessages.NameLengthValidation)]
    [RegularExpression(ConstantRegularExpressions.Name, ErrorMessage = ConstantMessages.NameFormatValidation)]
    string _firstName = null!;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(35, MinimumLength = 2, ErrorMessage = ConstantMessages.NameLengthValidation)]
    [RegularExpression(ConstantRegularExpressions.Name, ErrorMessage = ConstantMessages.NameFormatValidation)]
    string _lastName = null!;

    [Reactive]
    [RegularExpression(ConstantRegularExpressions.NotRequiredName, ErrorMessage = ConstantMessages.NotRequiredNameFormatValidation)]
    string? _patronymic;

    [Reactive]
    [DateTime]
    DateTimeOffset _birthday;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredRoleValidation)]
    UserRole? _role;

    [Reactive]
    [RequiredIf(nameof(UserId), 0, ErrorMessage = ConstantMessages.RequiredValidation)]
    [RegularExpression(ConstantRegularExpressions.Email, ErrorMessage = ConstantMessages.EmailFormatValidation)]
    string? _email;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    VerificationStatus _verificationStatus;

    [Reactive]
    [RequiredIf(nameof(UserId), 0, ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = ConstantMessages.PasswordLengthValidation)]
    string? _password;

    public static UserForm FromDto(UserDto dto)
    {
        return new UserForm
        {
            UserId = dto.UserId,
            FirstName = dto.Firstname,
            LastName = dto.Lastname,
            Patronymic = dto.Patronymic,
            Birthday = new DateTimeOffset(dto.Birthday.ToDateTime(TimeOnly.MinValue)),
            Role = dto.Role,
            VerificationStatus = dto.VerificationStatus,
        };
    }

    public UserDto ToDto()
    {
        return new UserDto(
            UserId,
            FirstName,
            LastName,
            Patronymic,
            DateOnly.FromDateTime(Birthday.DateTime),
            Role!.Value,
            VerificationStatus);
    }

    public UserForm Clone() => new()
    {
        UserId = UserId,
        FirstName = FirstName,
        LastName = LastName,
        Patronymic = Patronymic,
        Birthday = Birthday,
        Role = Role,
        Email = Email,
        VerificationStatus = VerificationStatus,
        Password = Password
    };

    public bool EqualsValues(UserForm? other)
    {
        if (other is null) return false;
        return UserId == other.UserId
            && FirstName == other.FirstName
            && LastName == other.LastName
            && Patronymic == other.Patronymic
            && Birthday == other.Birthday
            && Role == other.Role
            && VerificationStatus == other.VerificationStatus;
    }
}
