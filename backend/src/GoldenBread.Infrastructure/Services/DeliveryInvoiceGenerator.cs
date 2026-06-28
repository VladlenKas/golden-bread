using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GoldenBread.Infrastructure.Services;

public class DeliveryInvoiceGenerator : IDeliveryInvoiceGenerator
{
    public byte[] Generate(Order order, Company seller, User? user)
    {
        var today = DateTime.Now;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);

                page.Content().Column(col =>
                {
                    // Заголовок
                    col.Item().Text("НАКЛАДНАЯ")
                        .FontSize(14).Bold().AlignCenter();
                    col.Item().Text($"№ {order.OrderId} от {today:dd.MM.yyyy}")
                        .FontSize(12).AlignCenter();

                    col.Item().PaddingVertical(8);

                    // Блок контрагентов
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Продавец (отправитель)").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Покупатель (получатель)").Bold().AlignCenter();

                            static IContainer CellStyle(IContainer container) =>
                                container.Background(Colors.Grey.Lighten3).Border(1).Padding(4);
                        });

                        table.Cell().Element(CellStyle).Text($"Наименование: {seller?.Name}");
                        table.Cell().Element(CellStyle).Text($"Наименование: {order.Company?.Name}");
                        table.Cell().Element(CellStyle).Text($"ИНН: {seller?.Inn}");
                        table.Cell().Element(CellStyle).Text($"ИНН: {order.Company?.Inn}");
                        table.Cell().Element(CellStyle).Text($"ОГРН: {seller?.Ogrn}");
                        table.Cell().Element(CellStyle).Text($"ОГРН: {order.Company?.Ogrn}");
                        table.Cell().Element(CellStyle).Text($"Телефон: {seller?.Phone ?? "—"}");
                        table.Cell().Element(CellStyle).Text($"Телефон: {order.Company?.Phone ?? "—"}");
                        table.Cell().Element(CellStyle).Text($"Адрес: {seller?.Address ?? "—"}");
                        table.Cell().Element(CellStyle).Text($"Адрес: {order.Company?.Address ?? "—"}");

                        static IContainer CellStyle(IContainer container) =>
                            container.Border(1).Padding(4);
                    });

                    col.Item().PaddingVertical(8);

                    // Таблица позиций
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(2);
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("№").AlignCenter();
                            header.Cell().Element(HeaderStyle).Text("Товар").AlignCenter();
                            header.Cell().Element(HeaderStyle).Text("Кол-во партий").AlignCenter();
                            header.Cell().Element(HeaderStyle).Text("Ед. в партии").AlignCenter();
                            header.Cell().Element(HeaderStyle).Text("Цена за ед. (в руб.)").AlignCenter();
                            header.Cell().Element(HeaderStyle).Text("Всего единиц").AlignCenter();
                            header.Cell().Element(HeaderStyle).Text("Сумма (в руб.)").AlignCenter();

                            static IContainer HeaderStyle(IContainer container) =>
                                container.Background(Colors.Grey.Lighten3).Border(1).Padding(4);
                        });

                        int index = 1;
                        foreach (var item in order.OrderItems)
                        {
                            table.Cell().Element(CellStyle).Text((index++).ToString()).AlignCenter();
                            table.Cell().Element(CellStyle).Text(item.Batch?.Product?.Name ?? "—");
                            table.Cell().Element(CellStyle).Text($"{item.Quantity} шт.").AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{item.UnitsPerBatch} шт.").AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{item.UnitPrice:N2}").AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{item.TotalUnits} шт.").AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{item.TotalAmount:N2}").AlignRight();

                            static IContainer CellStyle(IContainer container) =>
                                container.Border(1).Padding(4);
                        }

                        table.Cell().ColumnSpan(5).Element(FooterStyle).Text("ИТОГО:").AlignRight().Bold();
                        table.Cell().Element(FooterStyle).Text($"{order.OrderItems.Sum(x => x.TotalUnits)} шт.").AlignCenter().Bold();
                        table.Cell().Element(FooterStyle).Text($"{order.OrderItems.Sum(x => x.TotalAmount):N2} руб.").AlignRight().Bold();

                        static IContainer FooterStyle(IContainer container) =>
                            container.Background(Colors.Grey.Lighten3).Border(1).Padding(4);
                    });

                    col.Item().PaddingVertical(8);

                    // Даты
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                        });

                        table.Cell().Text("Дата составления:").Bold();
                        table.Cell().Text(today.ToString("dd.MM.yyyy"));

                        table.Cell().Text("Дата отгрузки:").Bold();
                        table.Cell().Text(order.EndDate.ToString("dd.MM.yyyy"));

                        table.Cell().Text("Дата начала производства:").Bold();
                        table.Cell().Text(order.StartDate?.ToString("dd.MM.yyyy") ?? "—");

                        table.Cell().Text("Дата создания заказа:").Bold();
                        table.Cell().Text(order.CreatedAt.ToString("dd.MM.yyyy HH:mm"));

                        table.Cell().Text("Статус заказа:").Bold();
                        table.Cell().Text(StatusLocalized(order.Status));
                    });
                });
            });
        }).GeneratePdf();
    }
    
    private static string StatusLocalized(OrderStatus status) => status switch
    {
        OrderStatus.Created => "Создан",
        OrderStatus.InProgress => "В процессе",
        OrderStatus.Completed => "Выполнен",
        OrderStatus.Canceled => "Отменен",
        _ => "-"
    };
}