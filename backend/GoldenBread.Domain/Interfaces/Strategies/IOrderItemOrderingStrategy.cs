using GoldenBread.Domain.Entities;

namespace GoldenBread.Domain.Interfaces.Strategies;

public interface IOrderItemOrderingStrategy
{
    List<OrderItem> Sort(List<OrderItem> items);
}
