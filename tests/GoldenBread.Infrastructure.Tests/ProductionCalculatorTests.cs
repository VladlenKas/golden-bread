using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Services;
using Xunit;

namespace GoldenBread.Infrastructure.Tests;

public class ProductionCalculatorTests
{
    private readonly ProductionCalculator _calculator;

    public ProductionCalculatorTests()
    {
        _calculator = new ProductionCalculator();
    }

    [Fact]
    public void CalculatePlan_EmptyCart_ReturnsTomorrow()
    {
        var now = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc);
        var tariff = new OrderTariff { FreeEmployeesPercent = 100m };
        var employees = new List<Employee>();

        var result = _calculator.CalculatePlan(
            new List<CartItem>(), tariff, now, null, employees);

        Assert.Equal(new DateOnly(2024, 3, 16), result.MinimalDeliveryDate);
        Assert.Equal(new DateOnly(2024, 3, 16), result.ConfirmedDeliveryDate);
    }

    [Fact]
    public void CalculatePlan_SingleEmployee_100Percent()
    {
        var now = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc);
        var tariff = new OrderTariff { FreeEmployeesPercent = 100m };

        var product = CreateProduct(1, "Pie", 60); // 60 min per item
        var batch = CreateBatch(1, product, 2);    // 2 items per batch
        var cartItem = CreateCartItem(batch, 3);   // 3 batches = 6 items = 360 min

        var employees = new List<Employee>
        {
            CreateEmployee(1, "John", new List<EmployeeTask>())
        };

        var result = _calculator.CalculatePlan(
            new List<CartItem> { cartItem }, tariff, now, null, employees);

        // 360 min / 480 min per day = 1 day
        Assert.Single(result.SelectedEmployees);
        Assert.Equal(1, result.RequiredWorkDays);
        Assert.Equal(new DateOnly(2024, 3, 16), result.MinimalDeliveryDate);
    }

    [Fact]
    public void CalculatePlan_MultipleEmployees_50Percent()
    {
        var now = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc);
        var tariff = new OrderTariff { FreeEmployeesPercent = 50m }; // 50% of 4 = 2 employees

        var product = CreateProduct(1, "Pie", 120); // 120 min per item
        var batch = CreateBatch(1, product, 2);     // 2 items per batch = 240 min per batch
        var cartItem = CreateCartItem(batch, 4);    // 4 batches = 8 items = 960 min total

        var employees = new List<Employee>
        {
            CreateEmployee(1, "John", new List<EmployeeTask>()),    // Busy: 0
            CreateEmployee(2, "Jane", new List<EmployeeTask>()),    // Busy: 0
            CreateEmployee(3, "Bob", CreateTasks(240)),             // Busy: 240 min
            CreateEmployee(4, "Alice", CreateTasks(480))            // Busy: 480 min
        };

        var result = _calculator.CalculatePlan(
            new List<CartItem> { cartItem }, tariff, now, null, employees);

        // Should select 2 least busy: John and Jane
        Assert.Equal(2, result.SelectedEmployees.Count);
        Assert.Contains(result.SelectedEmployees, e => e.EmployeeId == 1);
        Assert.Contains(result.SelectedEmployees, e => e.EmployeeId == 2);

        // 960 min / 2 employees = 480 min each = 1 day
        Assert.Equal(1, result.RequiredWorkDays);
    }

    [Fact]
    public void CalculatePlan_RespectsDesiredDeliveryDate()
    {
        var now = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc);
        var desiredDate = new DateOnly(2024, 3, 25); // Far in future
        var tariff = new OrderTariff { FreeEmployeesPercent = 100m };

        var product = CreateProduct(1, "Pie", 60);
        var batch = CreateBatch(1, product, 2);
        var cartItem = CreateCartItem(batch, 3);

        var employees = new List<Employee> { CreateEmployee(1, "John", new List<EmployeeTask>()) };

        var result = _calculator.CalculatePlan(
            new List<CartItem> { cartItem }, tariff, now, desiredDate, employees);

        Assert.Equal(desiredDate, result.ConfirmedDeliveryDate);
        Assert.True(result.MinimalDeliveryDate < desiredDate);
    }

    [Fact]
    public void CalculatePlan_IgnoresTooEarlyDesiredDate()
    {
        var now = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc);
        var desiredDate = new DateOnly(2024, 3, 15); // Today - impossible
        var tariff = new OrderTariff { FreeEmployeesPercent = 100m };

        var product = CreateProduct(1, "Pie", 60);
        var batch = CreateBatch(1, product, 2);
        var cartItem = CreateCartItem(batch, 3);

        var employees = new List<Employee> { CreateEmployee(1, "John", new List<EmployeeTask>()) };

        var result = _calculator.CalculatePlan(
            new List<CartItem> { cartItem }, tariff, now, desiredDate, employees);

        Assert.Equal(result.MinimalDeliveryDate, result.ConfirmedDeliveryDate); // Uses minimal
        Assert.True(result.ConfirmedDeliveryDate > desiredDate);
    }

    // Helper methods
    private Product CreateProduct(int id, string name, int productionTimeMinutes)
    {
        var product = Product.Create(id, name, productionTimeMinutes, 100m);
        typeof(Product).GetProperty("ProductId")!.SetValue(product, id);
        return product;
    }

    private ProductBatch CreateBatch(int id, Product product, int quantityPerBatch)
    {
        var batch = ProductBatch.Create(id, product, quantityPerBatch, 20);
        typeof(ProductBatch).GetProperty("ProductBatchId")!.SetValue(batch, id);
        return batch;
    }

    private CartItem CreateCartItem(ProductBatch batch, int quantity)
    {
        var company = Company.Create("Test", "123", "123", Account.Create("test@test.com", "hash", AccountType.Company));
        var item = CartItem.Create(company, batch, quantity);
        return item;
    }

    private Employee CreateEmployee(int id, string name, List<EmployeeTask> tasks)
    {
        var employee = Employee.Create(id, name, "Doe", "Petrovich", new DateOnly(1990, 1, 1), tasks);
        return employee;
    }

    private List<EmployeeTask> CreateTasks(int busyMinutes)
    {
        var now = DateTime.UtcNow;
        return new List<EmployeeTask>
        {
            new EmployeeTask
            {
                StartTime = now.AddHours(-busyMinutes/60),
                EndTime = now
            }
        };
    }
}

