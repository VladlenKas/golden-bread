namespace GoldenBread.Domain.Entities;

public class OrderItem
{
    public int OrderItemId { get; private set; }

    public int OrderId { get; private set; }
    public int? BatchId { get; private set; }

    public int QuantityPerBatch { get; set; }
    public int UnitsInBatch { get; set; } // Старое количество продукций в партии. Просто для информации 
    public decimal UnitPriceAtOrder { get; set; }  // Старая цена продукции. Просто для информации 

    public ProductBatch Batch { get; set; } = null!;
    public Order Order { get; set; } = null!;

    public ICollection<EmployeeTask> EmployeeTasks { get; set; } = new List<EmployeeTask>();

    public OrderItem() { }

    public static OrderItem Create(
        int orderItemId,
        int orderId,
        int batchId,
        int quantity,
        int unitsInBatch,
        decimal unitPriceAtOrder)
    {
        return new OrderItem
        {
            OrderItemId = orderItemId,
            OrderId = orderId,
            BatchId = batchId,
            QuantityPerBatch = quantity,
            UnitsInBatch = unitsInBatch,
            UnitPriceAtOrder = unitPriceAtOrder
        };
    }

    public static OrderItem Create(
        int orderItemId,
        int orderId,
        ProductBatch batch,    
        int quantity,
        int unitsInBatch,
        decimal unitPriceAtOrder)
    {
        var item = Create(
            orderItemId, 
            orderId, 
            batch.ProductBatchId, 
            quantity, 
            unitsInBatch, 
            unitPriceAtOrder);

        item.Batch = batch;
        return item;
    }
}
