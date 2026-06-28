using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IDocumentsApi
{
    [Post("/api/document/delivery-invoice-pdf/{orderId}")]
    [Headers("Accept: application/pdf")]
    Task<IApiResponse<Stream>> DownloadDeliveryInvoiceAsync(int orderId, int userId);
}