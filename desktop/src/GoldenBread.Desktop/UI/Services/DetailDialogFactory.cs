using GoldenBread.Desktop.Features.Administration.Companies.Models;
using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.Common.DetailData;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;
using GoldenBread.Desktop.Features.Production.OrdersList.Dtos;
using GoldenBread.Desktop.Features.References.Employees.Models;
using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Features.References.Suppliers.Models;
using GoldenBread.Desktop.UI.Helpers;


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

    public static DetailDialogData FromOrder(OrderDetailResponse item)
    {
        return new DetailDialogData(
            Sections:
            [
                new DetailSectionData(
                Header: "Основная информация",
                Fields:
                [
                    new("Номер заказа", $"Заказ №{item.OrderId}"),
                    new("Компания", item.CompanyName),
                    new("Статус", GetStatusName(item.Status)),
                    new("Дата создания", item.CreatedAt.ToString("dd.MM.yyyy HH:mm")),
                    new("Дата начала", item.StartDate?.ToString("dd.MM.yyyy") ?? "Не назначена"),
                    new("Дата завершения", item.EndDate.ToString("dd.MM.yyyy")),
                    new("Общая сумма", $"{item.TotalAmount:N2} ₽")
                ]),

            new DetailSectionData(
                Header: $"Позиции заказа ({item.Items.Count})",
                Fields:
                [
                    ..item.Items.Select(i =>
                        new DetailFieldData(
                            $"Позиция",
                            $"Название: {i.ProductName} \nСумма: {i.TotalCost:N2} ₽ \nКол-во партий: {i.Quantity} \nКол-во ед. в партии: {i.BatchInfo} \nСтатус: ({GetStatusName(i.Status)})"))
                ]),

            new DetailSectionData(
                Header: $"Списанные ингредиенты ({item.Reservations.Count})",
                Fields: item.Reservations.Count != 0
                    ?
                    [
                        ..item.Reservations.Select(r =>
                            new DetailFieldData(
                                r.IngredientName,
                                $"{r.ReservedQuantity:N2} {LocalizedIngredientUnits.UnitsTable(r.Unit)} (партия от {r.DeliveryDate:dd.MM.yyyy}, годен до {r.ExpiryDate:dd.MM.yyyy})"))
                    ]
                    : [new DetailFieldData("Нет списанных ингредиентов", "")]),

            new DetailSectionData(
                Header: $"Задачи сотрудников ({item.Tasks.Count})",
                Fields: item.Tasks.Count != 0
                    ?
                    [
                        ..item.Tasks.Select(t =>
                            new DetailFieldData(
                                $"Задача №{t.EmployeeTaskId}",
                                $"Сотрудник: {t.EmployeeName} \nСлот: {t.StartTime:HH:mm}-{t.EndTime:HH:mm} \nСтатус: {GetStatusName(t.Status)}"))
                    ]
                    : [new DetailFieldData("Нет назначенных задач", "")])
            ]);
    }

    private static string GetStatusName(OrderStatus status) => status switch
    {
        OrderStatus.Created => "Создан",
        OrderStatus.InProgress => "В процессе",
        OrderStatus.Completed => "Готов",
        OrderStatus.Canceled => "Отменён",
        _ => status.ToString()
    };
}
