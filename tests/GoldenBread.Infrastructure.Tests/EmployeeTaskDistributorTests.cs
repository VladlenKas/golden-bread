using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.Services;
using Moq;
using Xunit;

namespace GoldenBread.Infrastructure.Tests;

public class EmployeeTaskDistributorTests
{
    private readonly Mock<IWorkScheduleCalculator> _scheduleMock;
    private readonly EmployeeTaskDistributor _distributor;

    public EmployeeTaskDistributorTests()
    {
        _scheduleMock = new Mock<IWorkScheduleCalculator>();
        _distributor = new EmployeeTaskDistributor(_scheduleMock.Object);
    }

    [Fact]
    public void Distribute_SingleEmployee_AllBatches()
    {
        // Arrange
        var orderItem = CreateOrderItem(1, 6, 2, 120); // 6 batches, 2 per batch, 120 min per item
        var employees = new List<Employee> { CreateEmployee(1, "John") };
        var availability = new Dictionary<int, DateTime> { { 1, DateTime.UtcNow } };

        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));

        // Act
        var result = _distributor.Distribute(orderItem, employees, availability, 100m);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result[0].EmployeeId);
        Assert.Equal(6, result[0].AssignedQuantity); // All 6 batches
        // 6 batches * 2 items * 120 min = 1440 min
        _scheduleMock.Verify(s => s.AddWorkMinutes(It.IsAny<DateTime>(), 1440), Times.Once);
    }

    [Fact]
    public void Distribute_TwoEmployees_EqualSplit()
    {
        // Arrange
        var orderItem = CreateOrderItem(1, 6, 2, 120); // 6 batches
        var employees = new List<Employee>
        {
            CreateEmployee(1, "John"),
            CreateEmployee(2, "Jane")
        };
        var availability = new Dictionary<int, DateTime>
        {
            { 1, new DateTime(2024, 3, 15, 9, 0, 0) },
            { 2, new DateTime(2024, 3, 15, 9, 0, 0) }
        };

        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));

        // Act
        var result = _distributor.Distribute(orderItem, employees, availability, 100m).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        // 6 batches / 2 employees = 3 each
        Assert.Equal(3, result[0].AssignedQuantity);
        Assert.Equal(3, result[1].AssignedQuantity);
        // 3 * 2 * 120 = 720 min each
        _scheduleMock.Verify(s => s.AddWorkMinutes(It.IsAny<DateTime>(), 720), Times.Exactly(2));
    }

    [Fact]
    public void Distribute_ThreeEmployees_SevenBatches_RoundUp()
    {
        // Arrange
        var orderItem = CreateOrderItem(1, 7, 2, 60); // 7 batches (odd number)
        var employees = new List<Employee>
        {
            CreateEmployee(1, "John"),
            CreateEmployee(2, "Jane"),
            CreateEmployee(3, "Bob")
        };
        var availability = new Dictionary<int, DateTime>
        {
            { 1, new DateTime(2024, 3, 15, 9, 0, 0) },
            { 2, new DateTime(2024, 3, 15, 9, 0, 0) },
            { 3, new DateTime(2024, 3, 15, 9, 0, 0) }
        };

        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));

        // Act
        var result = _distributor.Distribute(orderItem, employees, availability, 100m).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        // 7 batches / 3 = 2.33 → rounds up: 3, 2, 2 or 3, 3, 1
        Assert.Equal(7, result.Sum(r => r.AssignedQuantity));
        Assert.True(result.All(r => r.AssignedQuantity > 0));
    }

    [Fact]
    public void Distribute_50Percent_TakesHalfEmployees()
    {
        // Arrange
        var orderItem = CreateOrderItem(1, 10, 1, 60);
        var employees = new List<Employee>
        {
            CreateEmployee(1, "John"),
            CreateEmployee(2, "Jane"),
            CreateEmployee(3, "Bob"),
            CreateEmployee(4, "Alice")
        };
        var availability = new Dictionary<int, DateTime>
        {
            { 1, new DateTime(2024, 3, 15, 9, 0, 0) },
            { 2, new DateTime(2024, 3, 15, 9, 0, 0) },
            { 3, new DateTime(2024, 3, 15, 9, 0, 0) },
            { 4, new DateTime(2024, 3, 15, 9, 0, 0) }
        };

        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));

        // Act
        var result = _distributor.Distribute(orderItem, employees, availability, 50m).ToList();

        // Assert
        // 50% of 4 = 2 employees
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Distribute_UpdatesAvailability_SequentialTasks()
    {
        // Arrange
        var orderItem = CreateOrderItem(1, 4, 2, 60); // 4 batches, 2 items, 60 min = 480 min total
        var employees = new List<Employee> { CreateEmployee(1, "John") };
        var startTime = new DateTime(2024, 3, 15, 9, 0, 0);
        var availability = new Dictionary<int, DateTime> { { 1, startTime } };

        var endTime = startTime.AddHours(8);
        _scheduleMock.Setup(s => s.AddWorkMinutes(startTime, 480))
            .Returns(endTime);

        // Act - First distribution
        var result = _distributor.Distribute(orderItem, employees, availability, 100m).ToList();

        // Assert
        Assert.Single(result);

        // Second distribution should start after first ends
        var secondOrderItem = CreateOrderItem(2, 2, 1, 30);
        var secondResult = _distributor.Distribute(secondOrderItem, employees, availability, 100m).ToList();

        // Second task should start after first ends
        Assert.Equal(endTime, secondResult[0].StartTime);
    }

    [Fact]
    public void Distribute_EmptyEmployees_ReturnsEmpty()
    {
        // Arrange
        var orderItem = CreateOrderItem(1, 5, 2, 60);
        var availability = new Dictionary<int, DateTime>();

        // Act
        var result = _distributor.Distribute(orderItem, new List<Employee>(), availability, 100m);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Distribute_ZeroQuantity_ReturnsEmpty()
    {
        // Arrange
        var orderItem = CreateOrderItem(1, 0, 2, 60); // 0 batches
        var employees = new List<Employee> { CreateEmployee(1, "John") };
        var availability = new Dictionary<int, DateTime> { { 1, DateTime.UtcNow } };

        // Act
        var result = _distributor.Distribute(orderItem, employees, availability, 100m);

        // Assert
        Assert.Empty(result);
    }

    // Helper methods
    private OrderItem CreateOrderItem(int id, int quantity, int unitsInBatch, int productionTimeMinutes)
    {
        // Create product
        var product = Product.Create(1, "Test", productionTimeMinutes, 100m);
        typeof(Product).GetProperty("ProductId")!.SetValue(product, 1);

        // Create batch with product
        var batch = ProductBatch.Create(1, product, unitsInBatch, 20);
        typeof(ProductBatch).GetProperty("ProductBatchId")!.SetValue(batch, 1);
        // Product is already set in Create method

        // Create order item
        return OrderItem.Create(
            1, 1, batch, quantity, unitsInBatch, 100m);
    }

    private Employee CreateEmployee(int id, string name)
    {
        return Employee.Create(id, name, "Doe", "Petrovich", new DateOnly(1990, 1, 1), null);
    }
}

