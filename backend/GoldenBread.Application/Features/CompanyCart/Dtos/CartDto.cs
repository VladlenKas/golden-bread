namespace GoldenBread.Application.Features.CompanyCart.Dtos;

public class CartDto
{
    public List<ProductCartItemDto> CartItemsList { get; set; } = null!;
    public DateOnly MinimalDeliveryDate { get; set; }
    public DateOnly SelectedDeliveryDate { get; set; }
}

public class ProductCartItemDto
{
    // Основные данные
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public int ProductionTimeMinutes { get; set; }

    // Выбранная партия
    public int ProductBatchId { get; set; }
    public int QuantityPerBatch { get; set; }

    // Первое изображение
    public string? ImageUrl { get; set; }

    // Флаги / Вычисляемые свойства
    public bool IsFavorite { get; set; }
    public int QuantityInCart { get; set; }
    public decimal TotalCostInCart { get; set; }
}
