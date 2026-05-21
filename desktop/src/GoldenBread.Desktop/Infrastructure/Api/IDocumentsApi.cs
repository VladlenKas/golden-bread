using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IDocumentsApi
{
    [Post("/api/document/delivery-invoice-xlsx/{orderId}")]
    [Headers("Accept: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Task<IApiResponse<Stream>> DownloadDeliveryInvoiceAsync(int orderId);
}