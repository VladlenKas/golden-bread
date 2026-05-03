using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.Infrastructure.Helpers;
using GoldenBread.Desktop.UI.Common;
using ReactiveUI.SourceGenerators;
using System.ComponentModel.DataAnnotations;
using Tmds.DBus.Protocol;

namespace GoldenBread.Desktop.Features.References.Suppliers.Models;

public partial class SupplierForm : ViewModelBase
{
    [Reactive]
    int _supplierId;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ConstantMessages.SupplierNameLengthValidation)]
    [RegularExpression(ConstantRegularExpressions.SupplierName, ErrorMessage = ConstantMessages.SupplierNameFormatValidation)]
    string _name = null!;

    [Reactive]
    [RegularExpression(ConstantRegularExpressions.Email, ErrorMessage = ConstantMessages.EmailFormatValidation)]
    string? _email;

    [Reactive]
    [RegularExpression(ConstantRegularExpressions.Phone, ErrorMessage = ConstantMessages.PhoneFormatValidation)]
    string? _phone;

    [Reactive]
    [StringLength(200, ErrorMessage = ConstantMessages.AddressLengthValidation)]
    string? _address;

    public static SupplierForm FromDto(SupplierDto dto)
    {
        return new SupplierForm
        {
            SupplierId = dto.SupplierId,
            Name = dto.Name,
            Email = dto.Email,
            Phone = InputFormatter.FormatPhone(dto.Phone),
            Address = dto.Address
        };
    }

    public SupplierDto ToDto()
    {
        return new SupplierDto(
            SupplierId,
            Name,
            Email,
            InputFormatter.NormalizePhone(Phone),
            Address);
    }

    public SupplierForm Clone() => new()
    {
        SupplierId = SupplierId,
        Name = Name,
        Email = Email,
        Phone = Phone,
        Address = Address
    };

    public bool EqualsValues(SupplierForm? other)
    {
        if (other is null) return false;
        return SupplierId == other.SupplierId
            && Name == other.Name
            && Email == other.Email
            && Phone == other.Phone
            && Address == other.Address;
    }
}