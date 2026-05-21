using ClosedXML.Excel;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Services;

public class DeliveryInvoiceGenerator : IDeliveryInvoiceGenerator
{
    public byte[] Generate(Order order, Company seller)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Накладная");

        ConfigureSheet(ws);

        int row = 2;

        // Заголовок
        ws.Range(row, 1, row, 8).Merge();
        ws.Cell(row, 1).Value = $"НАКЛАДНАЯ НА ПОСТАВЛЯЕМУЮ ПРОДУКЦИЮ №{order.OrderId} ОТ {order.EndDate:dd.MM.yyyy}";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Font.FontSize = 12;
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(row, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        row += 2;

        // Блок контрагентов
        AddCounterpartyBlock(ws, ref row, seller, order.Company);

        row++;

        // Таблица позиций
        row = AddItemsTable(ws, row, order.OrderItems.ToList());

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private void ConfigureSheet(IXLWorksheet ws)
    {
        ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
        ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
        ws.PageSetup.Margins.Left = 0.3;
        ws.PageSetup.Margins.Right = 0.3;
        ws.PageSetup.Margins.Top = 0.4;
        ws.PageSetup.Margins.Bottom = 0.4;
        ws.PageSetup.CenterHorizontally = true;

        ws.Column(1).Width = 7;
        ws.Column(2).Width = 18;
        ws.Column(3).Width = 12;
        ws.Column(4).Width = 14;
        ws.Column(5).Width = 14;
        ws.Column(6).Width = 14;
        ws.Column(7).Width = 14;
        ws.Column(8).Width = 14;

        ws.Style.Font.FontName = "Arial";
        ws.Style.Font.FontSize = 10;
    }

    private void AddCounterpartyBlock(IXLWorksheet ws, ref int row, Company seller, Company buyer)
    {
        ws.Range(row, 1, row, 4).Merge();
        ws.Cell(row, 1).Value = "Продавец (отправитель)";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        ws.Cell(row, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        ws.Range(row, 5, row, 8).Merge();
        ws.Cell(row, 5).Value = "Покупатель (получатель)";
        ws.Cell(row, 5).Style.Font.Bold = true;
        ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(row, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
        ws.Cell(row, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        row++;

        AddCounterpartyRow(ws, row, "Наименование", seller?.Name, buyer?.Name); row++;
        AddCounterpartyRow(ws, row, "ИНН", seller?.Inn, buyer?.Inn); row++;
        AddCounterpartyRow(ws, row, "ОГРН", seller?.Ogrn, buyer?.Ogrn); row++;
        AddCounterpartyRow(ws, row, "Телефон", seller?.Phone ?? "", buyer?.Phone ?? ""); row++;
        AddCounterpartyRow(ws, row, "Эл. почта", seller?.Account?.Email ?? "", buyer?.Account?.Email ?? ""); row++;
        AddCounterpartyRow(ws, row, "Адрес", seller?.Address ?? "", buyer?.Address ?? ""); row++;
    }

    private void AddCounterpartyRow(IXLWorksheet ws, int row, string label, string sellerValue, string buyerValue)
    {
        ws.Cell(row, 1).Value = label;
        ws.Range(row, 1, row, 1).Style.Font.Bold = true;

        ws.Range(row, 2, row, 4).Merge();
        ws.Cell(row, 2).Value = sellerValue;

        ws.Cell(row, 5).Value = label;
        ws.Range(row, 5, row, 5).Style.Font.Bold = true;

        ws.Range(row, 6, row, 8).Merge();
        ws.Cell(row, 6).Value = buyerValue;

        for (int col = 1; col <= 8; col++)
            ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    }

    private int AddItemsTable(IXLWorksheet ws, int startRow, List<OrderItem> items)
    {
        int row = startRow;

        ws.Range(row, 1, row, 8).Merge();
        ws.Cell(row, 1).Value = "Позиции накладной";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        ws.Cell(row, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        row++;

        string[] headers =
        {
            "№", "Товар", "Ед. изм.", "Кол-во партий",
            "Ед. в партии", "Всего единиц", "Цена за ед., ₽", "Сумма, ₽"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(row, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }
        row++;

        int index = 1;
        foreach (var item in items)
        {
            ws.Cell(row, 1).Value = index++;
            ws.Cell(row, 2).Value = item.Batch?.Product?.Name ?? "—";
            ws.Cell(row, 3).Value = "шт.";
            ws.Cell(row, 4).Value = item.Quantity;
            ws.Cell(row, 5).Value = item.UnitsPerBatch;
            ws.Cell(row, 6).Value = item.TotalUnits;
            ws.Cell(row, 7).Value = item.UnitPrice;
            ws.Cell(row, 8).Value = item.TotalAmount;

            for (int col = 1; col <= 8; col++)
                ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";

            row++;
        }

        var totalUnits = items.Sum(x => x.TotalUnits);
        var totalAmount = items.Sum(x => x.TotalAmount);
        var totalUnitPrice = items.Sum(x => x.UnitPrice);

        ws.Range(row, 1, row, 5).Merge();
        ws.Cell(row, 1).Value = "ИТОГО:";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        ws.Cell(row, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        ws.Cell(row, 6).Value = totalUnits;
        ws.Cell(row, 7).Value = totalUnitPrice;
        ws.Cell(row, 8).Value = totalAmount;

        ws.Cell(row, 6).Style.Font.Bold = true;
        ws.Cell(row, 8).Style.Font.Bold = true;
        ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
        ws.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";

        for (int col = 1; col <= 8; col++)
            ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        return row + 1;
    }
}
