using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Strategies;

namespace GoldenBread.Application.Common.Strategies.Product;

public class ShortestShelfLifeStrategy : IOrderItemOrderingStrategy
{
    public List<OrderItem> Sort(List<OrderItem> items)
        => items.OrderBy(e => e.Batch.Product.ShelfLifeDays).ToList();
}
