using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GoldenBread.Infrastructure.Services;

public class CooperationAgreementGenerator : ICooperationAgreementGenerator
{
    public byte[] Generate(Company buyer)
    {
        var today = DateTime.Now;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.Content().Column(col =>
                {
                    col.Item().Text("УСЛОВИЯ СОТРУДНИЧЕСТВА")
                        .FontSize(16).Bold().AlignCenter();
                    col.Item().Text("с ООО «GoldenBread»")
                        .FontSize(12).AlignCenter();
                    col.Item().PaddingVertical(4);
                    col.Item().Text($"Документ сформирован: {today:dd.MM.yyyy}")
                        .FontSize(10).AlignRight().FontColor(Colors.Grey.Medium);
                    col.Item().PaddingVertical(8);

                    col.Item().Background(Colors.Grey.Lighten4).Border(1).Padding(8).Column(info =>
                    {
                        info.Item().Text("ДАННЫЕ ПОКУПАТЕЛЯ").Bold().FontSize(11);
                        info.Item().PaddingVertical(2);
                        info.Item().Text($"Организация: {buyer.Name}").FontSize(10);
                        info.Item().Text($"ИНН: {buyer.Inn}").FontSize(10);
                        info.Item().Text($"ОГРН: {buyer.Ogrn}").FontSize(10);
                        info.Item().Text($"Адрес: {buyer.Address ?? "-"}").FontSize(10);
                        info.Item().Text($"Телефон: {buyer.Phone ?? "-"}").FontSize(10);
                    });
                    col.Item().PaddingVertical(8);

                    col.Item().Text("1. ПРЕДМЕТ СОТРУДНИЧЕСТВА").Bold().FontSize(12);
                    col.Item().Text("1.1. Поставщик производит и поставляет хлебобулочную продукцию.").FontSize(10);
                    col.Item().Text("1.2. Заказы размещаются через информационную систему.").FontSize(10);
                    col.Item().PaddingVertical(4);

                    col.Item().Text("2. ПОРЯДОК РАБОТЫ").Bold().FontSize(12);
                    col.Item().Text("2.1. Сроки изготовления рассчитываются автоматически.").FontSize(10);
                    col.Item().Text("2.2. Доставка осуществляется силами Поставщика.").FontSize(10);
                    col.Item().PaddingVertical(4);

                    col.Item().Text("3. ОПЛАТА").Bold().FontSize(12);
                    col.Item().Text("3.1. Расчёты безналичным переводом по выставленному счёту либо наличной оплатой перед завершением сделки.").FontSize(10);
                    col.Item().PaddingVertical(4);

                    col.Item().Text("4. КАЧЕСТВО").Bold().FontSize(12);
                    col.Item().Text("4.1. Продукция соответствует ГОСТ и санитарным нормам.").FontSize(10);
                    col.Item().Text("4.2. Претензии принимаются в течение 24 часов с момента доставки.").FontSize(10);
                    col.Item().PaddingVertical(4);

                    col.Item().Text("5. КОНТАКТЫ").Bold().FontSize(12);
                    col.Item().Text("ООО «GoldenBread»").FontSize(10).Bold();
                    col.Item().Text("Адрес: г. Уфа, ул. Кирова, 65/2").FontSize(10);
                    col.Item().Text("Телефон: 8 (937) 788-80-90").FontSize(10);
                    col.Item().PaddingVertical(8);

                    col.Item().Background(Colors.Grey.Lighten4).Border(1).Padding(12).Column(confirm =>
                    {
                        confirm.Item().Text("ПОДТВЕРЖДЕНИЕ").Bold().FontSize(11).AlignCenter();
                        confirm.Item().PaddingVertical(4);
                        confirm.Item().Text($"Настоящим подтверждается, что {buyer.Name} (ИНН: {buyer.Inn}) ознакомлен(а) с условиями и принимает их.")
                            .FontSize(10).AlignCenter();
                        confirm.Item().PaddingVertical(4);
                        confirm.Item().Text($"Дата: {today:dd.MM.yyyy}").FontSize(10).AlignCenter();
                        confirm.Item().Text("Подтверждение осуществлено электронно через информационную систему.")
                            .FontSize(9).Italic().FontColor(Colors.Grey.Medium).AlignCenter();
                    });
                });
            });
        }).GeneratePdf();
    }
}