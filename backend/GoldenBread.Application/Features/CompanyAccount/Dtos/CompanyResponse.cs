using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.CompanyAccount.Dtos;

public sealed class CompanyResponse
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Inn { get; set; } = null!;
    public string Ogrn { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public ICollection<OrderResponse> Orders { get; set; } = new List<OrderResponse>();
}

public sealed class OrderResponse
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; } 
    public OrderTariffResponse Tariff { get; set; } = null!;
}

public sealed class OrderTariffResponse
{
    public string Name { get; set; } = null!;
}