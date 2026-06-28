namespace GoldenBread.Application.Features.Document.Dtos;

public class GenerateDeliveryInvoiceResponse
{
    public byte[] FileBytes { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = "application/pdf";
}
