using GoldenBread.Desktop.Features.Administration.Companies.Models;
using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.Features.Common.Models;
using GoldenBread.Desktop.Features.References.Employees.Models;
using GoldenBread.Desktop.Features.References.Suppliers.Models;


namespace GoldenBread.Desktop.UI.Services;

public static class DetailDialogFactory
{
    public static DetailDialogData FromUser(UserListItem item)
    {
        return new DetailDialogData(
            Sections:
            [
                new DetailSectionData(
                    Header: "Персональные данные",
                    Fields:
                    [
                        new("ФИО", item.Fullname),
                        new("Дата рождения", item.Birthday.ToString("dd.MM.yyyy")),
                        new("Роль", item.LocalizedRole),
                    ]),
                new DetailSectionData(
                    Header: "Учётные данные",
                    Fields:
                    [
                        new("Эл. почта", item.Email),
                        new("Статус", item.LocalizedStatus),
                        new("Аккаунт создан", item.CreatedAtFormatted),
                        new("Сессия актвина до", item.SessionExpiresAtFormatted)
                    ])
            ]);
    }

    public static DetailDialogData FromCompany(CompanyListItem item)
    {
        return new DetailDialogData(
            Sections:
            [
                new DetailSectionData(
                    Header: "Реквизиты",
                    Fields:
                    [
                        new("Название", item.Name),
                        new("ИНН", item.InnFormatted),
                        new("ОГРН", item.OgrnFormatted),
                        new("Телефон", item.PhoneFormatted),
                        new("Адрес", item.AddressFormatted)
                    ]),
                new DetailSectionData(
                    Header: "Учётные данные",
                    Fields:
                    [
                        new("Эл. почта", item.Email),
                        new("Статус", item.LocalizedStatus),
                        new("Аккаунт создан", item.CreatedAtFormatted),
                        new("Сессия актвина до", item.SessionExpiresAtFormatted)
                    ])
            ]);
    }

    public static DetailDialogData FromEmployee(EmployeeListItem item)
    {
        return new DetailDialogData(
            Sections:
            [
                new DetailSectionData(
                    Header: "Персональные данные",
                    Fields:
                    [
                        new("ФИО", item.Fullname),
                        new("Дата рождения", item.Birthday.ToString("dd.MM.yyyy"))
                    ])
            ]);
    }

    public static DetailDialogData FromSupplier(SupplierListItem item)
    {
        return new DetailDialogData(
            Sections:
            [
                new DetailSectionData(
                    Header: "Контактные данные",
                    Fields:
                    [
                        new("Название", item.Name),
                        new("Телефон", item.PhoneFormatted),
                        new("Эл. почта", item.EmailFormatted),
                        new("Адрес", item.AddressFormatted)
                    ])
            ]);
    }
}
