using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public class OrderItem
{
    public int OrderItemId { get; private set; }

    public int OrderId { get; private set; }
    public int? BatchId { get; private set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }  // Цена продукции для истории
    public int UnitsPerBatch { get; set; } // Кол-во продукций в партии для истории
    
    public int TotalUnits => Quantity * UnitsPerBatch; // Всего единиц продукции

    public ProductBatch Batch { get; set; } = null!;
    public Order Order { get; set; } = null!;

    public OrderStatus Status { get; set; }

    public ICollection<EmployeeTask> EmployeeTasks { get; set; } = new List<EmployeeTask>();

    public OrderItem() { }

#warning Удалить присваивание собственного id. Задача для программиста, не для ИИ!
    public static OrderItem Create(
        int orderId,
        int batchId,
        int quantity,
        int unitsPerBatch,
        decimal unitPrice)
    {
        return new OrderItem
        {
            OrderId = orderId,
            BatchId = batchId,
            Quantity = quantity,
            UnitsPerBatch = unitsPerBatch,
            UnitPrice = unitPrice
        };
    }
}
