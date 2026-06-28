using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.Document.Dtos;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Document.Commands;

public class GenerateDeliveryInvoiceCommandHandler(
    IUserRepository userRepository,
    IOrderRepository orderRepository,
    IDeliveryInvoiceGenerator invoiceGenerator) :
    IRequestHandler<GenerateDeliveryInvoiceCommand, GenerateDeliveryInvoiceResponse>
{
    public async Task<GenerateDeliveryInvoiceResponse> Handle(
        GenerateDeliveryInvoiceCommand command,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.IssuedByUserId, ct);

        // 1. Получить заказ со всеми связями
        var order = await orderRepository.GetByIdAsync(command.OrderId, ct) 
            ?? throw new InvalidOperationException("Заказ не найден");

        // 2. Получить данные продавца 
        var company = Company.Create(
            0,
            0,
            "GoldenBread",
            "529402859238",
            "1857203028347",
            "89377888090",
            "г. Уфа, Кирова 65/2, УКСИВТ");

        // 3. Генерируем док 
        var fileBytes = invoiceGenerator.Generate(order, company, user);

        return new GenerateDeliveryInvoiceResponse
        {
            FileBytes = fileBytes,
            FileName = $"Накладная_№{order.OrderId}_от_{DateTime.Now:dd.MM.yyyy}.pdf"
        };
    }
}
