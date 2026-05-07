using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.Infrastructure.Helpers;
using GoldenBread.Desktop.UI.Common;
using System.ComponentModel.DataAnnotations;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.Features.Administration.Companies.Models;

public partial class CompanyForm : ViewModelBase
{
    [Reactive]
    int _companyId;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ConstantMessages.CompanyNameLengthValidation)]
    string _name = null!;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [RegularExpression(ConstantRegularExpressions.Inn, ErrorMessage = ConstantMessages.InnFormatValidation)]
    string _inn = null!;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [RegularExpression(ConstantRegularExpressions.Ogrn, ErrorMessage = ConstantMessages.OgrnFormatValidation)]
    string _ogrn = null!;

    [Reactive]
    [RegularExpression(ConstantRegularExpressions.Phone, ErrorMessage = ConstantMessages.PhoneFormatValidation)]
    string? _phone;

    [Reactive]
    [StringLength(200, ErrorMessage = ConstantMessages.AddressLengthValidation)]
    string? _address;

    [Reactive]
    [RequiredIf(nameof(CompanyId), 0, ErrorMessage = ConstantMessages.RequiredValidation)]
    [RegularExpression(ConstantRegularExpressions.Email, ErrorMessage = ConstantMessages.EmailFormatValidation)]
    string? _email;

    [Reactive]
    [RequiredIf(nameof(CompanyId), 0, ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = ConstantMessages.PasswordLengthValidation)]
    string? _password;

    public static CompanyForm FromDto(CompanyDto dto)
    {
        return new CompanyForm
        {
            CompanyId = dto.CompanyId,
            Name = dto.Name,
            Inn = InputFormatter.FormatInn(dto.Inn) ?? dto.Inn,
            Ogrn = InputFormatter.FormatOgrn(dto.Ogrn) ?? dto.Ogrn,
            Phone = InputFormatter.FormatPhone(dto.Phone),
            Address = dto.Address
        };
    }

    public CompanyDto ToDto()
    {
        return new CompanyDto(
            CompanyId,
            Name,
            InputFormatter.NormalizeInn(Inn) ?? Inn,
            InputFormatter.NormalizeOgrn(Ogrn) ?? Ogrn,
            InputFormatter.NormalizePhone(Phone),
            Address);
    }

    public CompanyForm Clone() => new()
    {
        CompanyId = CompanyId,
        Name = Name,
        Inn = Inn,
        Ogrn = Ogrn,
        Phone = Phone,
        Address = Address,
        Email = Email,
        Password = Password
    };

    public bool EqualsValues(CompanyForm? other)
    {
        if (other is null) return false;
        return CompanyId == other.CompanyId
            && Name == other.Name
            && Inn == other.Inn
            && Ogrn == other.Ogrn
            && Phone == other.Phone
            && Address == other.Address;
    }
}