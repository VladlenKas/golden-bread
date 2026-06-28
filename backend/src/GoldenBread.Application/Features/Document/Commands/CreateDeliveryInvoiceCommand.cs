using GoldenBread.Application.Features.CompanyOrder.Dtos;
using GoldenBread.Application.Features.Document.Dtos;

namespace GoldenBread.Application.Features.Document.Commands;

public sealed record GenerateDeliveryInvoiceCommand(int OrderId, int IssuedByUserId)
    : IRequest<GenerateDeliveryInvoiceResponse>;
