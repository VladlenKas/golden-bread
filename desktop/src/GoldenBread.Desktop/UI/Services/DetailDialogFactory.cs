using GoldenBread.Desktop.Features.Administration.Companies.Models;
using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.Features.Common.DetailData;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;
using GoldenBread.Desktop.Features.References.Employees.Models;
using GoldenBread.Desktop.Features.References.Products.Models;
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
                        new("Должность", item.LocalizedRole),
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

    public static DetailDialogData FromPurchasePosition(SupplierIngredientListItem item)
    {
        return new DetailDialogData(
            Sections:
            [
                new DetailSectionData(
                    Header: "Информация о товаре",
                    Fields:
                    [
                        new("Наименование", item.Name),
                        new("Поставщик", item.SupplierName),
                        new("Тип ингредиента", item.IngredientName)
                    ]),

                new DetailSectionData(
                    Header: "Характеристики",
                    Fields:
                    [
                        new("Вес/Объем", item.WeightFormatted),
                        new("Цена закупки", item.PriceFormatted),
                        new("Срок хранения", item.ShelfLifeFormatted)
                    ]),

                new DetailSectionData(
                    Header: "Складские остатки",
                    Fields:
                    [
                        new("Количество партий", item.QuantityBatchesFormatted),
                        new("Остаток на складе", item.QuantityUnitInBatchesFormatted)
                    ])
            ]);
    }

    public static DetailDialogData FromProduct(ProductDetailResponse item)
    {
        return new DetailDialogData(
            Sections:
            [
                new DetailSectionData(
                Header: "Основная информация",
                Fields:
                [
                    new("Название", item.Name),
                    new("Описание", item.Description),
                    new("Вес", $"{item.Weight} г"),
                    new("Время приготовления", $"{item.ProductionTimeMinutes} мин"),
                    new("Срок хранения", $"{item.ShelfLifeDays} дней"),
                    new("Температура хранения", $"от {item.StorageTempMin}°C до {item.StorageTempMax}°C"),
                    new("Категория", item.CategoryName)
                ]),

            new DetailSectionData(
                Header: "Рецепт",
                Fields:
                [
                    ..item.Ingredients.Select(i =>
                        new DetailFieldData(i.Name, $"{i.Quantity} {i.UnitLocalized}"))
                ]),

            new DetailSectionData(
                Header: "Партии продажи",
                Fields:
                [
                    ..item.AvailableBatches.Select(b =>
                        new DetailFieldData(
                            $"{b.QuantityPerBatch} шт",
                            $"{b.UnitPrice:N2} ₽ / шт (всего {b.TotalPrice:N2} ₽)"))
                ])
            ]);
    }
}
