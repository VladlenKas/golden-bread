using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IDocumentsApi
{
    // Если BaseAddress твоего HttpClient уже содержит .../api/
    [Post("/api/document/delivery-invoice-xlsx/{orderId}")]
    [Headers("Accept: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Task<IApiResponse<Stream>> DownloadDeliveryInvoiceAsync(int orderId);
}