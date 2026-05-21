using GoldenBread.Desktop.Features.Production.OrdersList.Dtos;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IOrdersApi
{
    [Get("/api/orders/kanban")]
    Task<IApiResponse<List<OrderKanbanItem>>> GetKanban();

    [Put("/api/orders/status")]
    Task<IApiResponse> UpdateStatus([Body] UpdateOrderStatusRequest request);

    [Get("/api/orders/editor-data")]
    Task<IApiResponse<OrderEditorDataResponse>> GetEditorData();

    [Post("/api/orders/calculate-delivery")]
    Task<IApiResponse<CalculateDeliveryResponse>> CalculateDelivery([Body] CalculateDeliveryRequest request);

    [Post("/api/orders/create-from-user")]
    Task<IApiResponse<int>> Create([Body] CreateOrderRequest request);

    [Get("/api/orders/{id}")]
    Task<IApiResponse<OrderDetailResponse>> GetById(int id);
}