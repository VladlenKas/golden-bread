using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.CompanyOrder.Commands.CreateOrder;
using GoldenBread.Application.Features.CompanyOrder.Services;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Interfaces.Services;
using Moq;
using Xunit;

namespace GoldenBread.Infrastructure.Tests;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IEmployeeTaskRepository> _employeeTaskRepositoryMock;
    private readonly Mock<IOrderTariffRepository> _tariffRepositoryMock;
    private readonly Mock<IProductionCalculator> _productionCalculatorMock;
    private readonly Mock<IIngredientReservationService> _ingredientServiceMock;
    private readonly Mock<IEmployeeTaskDistributor> _taskDistributorMock;
    private readonly Mock<IWorkScheduleCalculator> _workScheduleCalculator;
    private readonly Mock<ICurrentAccountContext> _accountContext;

    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderItemRepositoryMock = new Mock<IOrderItemRepository>();
        _cartRepositoryMock = new Mock<ICartRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _employeeTaskRepositoryMock = new Mock<IEmployeeTaskRepository>();
        _tariffRepositoryMock = new Mock<IOrderTariffRepository>();
        _productionCalculatorMock = new Mock<IProductionCalculator>();
        _ingredientServiceMock = new Mock<IIngredientReservationService>();
        _taskDistributorMock = new Mock<IEmployeeTaskDistributor>();
        _workScheduleCalculator = new Mock<IWorkScheduleCalculator>();
        _accountContext = new Mock<ICurrentAccountContext>();

        _handler = new CreateOrderCommandHandler(
            accountContext: _accountContext.Object,
            orderRepository: _orderRepositoryMock.Object,
            orderItemRepository: _orderItemRepositoryMock.Object,
            cartRepository: _cartRepositoryMock.Object,
            employeeRepository: _employeeRepositoryMock.Object,
            employeeTaskRepository: _employeeTaskRepositoryMock.Object,
            tariffRepository: _tariffRepositoryMock.Object,
            productionCalculator: _productionCalculatorMock.Object,
            ingredientService: _ingredientServiceMock.Object,
            taskDistributor: _taskDistributorMock.Object,
            workScheduleCalculator: _workScheduleCalculator.Object);

    }

    [Fact]
    public async Task Handle_IngredientsSufficient_CreatesOrderSuccessfully()
    {
        // Arrange
        int companyId = await _accountContext.Object.GetCompanyIdAsync(CancellationToken.None);
        var tariffId = 1;
        var desiredDeliveryDate = new DateOnly(2024, 3, 15);
        var command = new CreateOrderCommand(desiredDeliveryDate, tariffId);

        // Cart items
        var product = Product.Create(1, "Apple Pie", 120, 100m);
        typeof(Product).GetProperty("ProductId")!.SetValue(product, 1);

        var batch = ProductBatch.Create(1, product, 2, 20); // 2 items per batch, 20% markup
        typeof(ProductBatch).GetProperty("ProductBatchId")!.SetValue(batch, 1);

        var cartItems = new List<CartItem>
        {
            CartItem.Create(
                Company.Create("Test Co", "1234567890", "1234567890123", Account.Create("test@test.com", "hash", AccountType.Company)),
                batch,
                6) // 6 batches = 12 items
        };
        typeof(CartItem).GetProperty("CartItemId")!.SetValue(cartItems[0], 1);

        _cartRepositoryMock
            .Setup(x => x.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cartItems);

        // Tariff
        var tariff = new OrderTariff
        {
            OrderTariffId = tariffId,
            Name = "Standard",
            MarkupPercent = 10m,
            FreeEmployeesPercent = 50m
        };
        _tariffRepositoryMock
            .Setup(x => x.GetByIdAsync(tariffId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tariff);

        // Employees
        var employees = new List<Employee>
        {
            Employee.Create(1, "John", "Doe", "Petrovich", new DateOnly(1990, 1, 1), null)
        };
        _employeeRepositoryMock
            .Setup(x => x.GetActiveWithTasksAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        // Production plan
        var productionPlan = new ProductionPlan(
            new DateOnly(2024, 3, 14),
            desiredDeliveryDate,
            new DateTime(2024, 3, 13, 9, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 3, 15, 18, 0, 0, DateTimeKind.Utc),
            2,
            employees);

        _productionCalculatorMock
            .Setup(x => x.CalculatePlan(cartItems, tariff, It.IsAny<DateTime>(), desiredDeliveryDate, employees))
            .Returns(productionPlan);

        // Ingredient check - SUFFICIENT
        var ingredientCheck = new IngredientCheckResult(
            true, // Sufficient!
            new List<IngredientRequirement>
            {
                new(1, "Flour", 1000m, 1500m, 1000m),
                new(2, "Apples", 2000m, 2500m, 2000m)
            },
            new List<IngredientRequirement>());

        _ingredientServiceMock
            .Setup(x => x.CheckAsync(It.IsAny<IReadOnlyList<OrderItem>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientCheck);

        // Order creation
        var createdOrder = Order.Create(companyId, tariffId, OrderStatus.Awaiting, desiredDeliveryDate);
        typeof(Order).GetProperty("OrderId")!.SetValue(createdOrder, 123);

        _orderRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        // Task distribution
        var taskAssignments = new List<EmployeeTaskAssignment>
        {
            new(1, 0, new DateTime(2024, 3, 13, 9, 0, 0), new DateTime(2024, 3, 13, 17, 0, 0), 6)
        };
        _taskDistributorMock
            .Setup(x => x.Distribute(It.IsAny<OrderItem>(), It.IsAny<IReadOnlyList<Employee>>(),
                It.IsAny<Dictionary<int, DateTime>>(), 50m))
            .Returns(taskAssignments);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(123, result.OrderId);
        Assert.Equal(desiredDeliveryDate, result.DeliveryDate);
        Assert.False(result.IsDeferred);
        Assert.False(result.InsufficientIngredients);
        Assert.Empty(result.Deficits);

        // Verify calls
        _orderRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _orderItemRepositoryMock.Verify(x => x.CreateRangeAsync(It.IsAny<IEnumerable<OrderItem>>(), It.IsAny<CancellationToken>()), Times.Once);
        _ingredientServiceMock.Verify(x => x.ReserveForOrderAsync(It.IsAny<IReadOnlyList<OrderItem>>(), 123, true, It.IsAny<CancellationToken>()), Times.Once);
        _employeeTaskRepositoryMock.Verify(x => x.BulkCreateAsync(It.IsAny<IEnumerable<EmployeeTask>>(), It.IsAny<CancellationToken>()), Times.Once);
        _cartRepositoryMock.Verify(x => x.ClearAsync(companyId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_IngredientsInsufficient_ReturnsDeficitInfo()
    {
        // Arrange
        int companyId = await _accountContext.Object.GetCompanyIdAsync(CancellationToken.None);
        var tariffId = 1;
        var desiredDeliveryDate = new DateOnly(2024, 3, 15);
        var command = new CreateOrderCommand(desiredDeliveryDate, tariffId);

        // Cart items
        var product = Product.Create(1, "Apple Pie", 120, 100m);
        typeof(Product).GetProperty("ProductId")!.SetValue(product, 1);

        var batch = ProductBatch.Create(1, product, 2, 20);
        typeof(ProductBatch).GetProperty("ProductBatchId")!.SetValue(batch, 1);

        var cartItems = new List<CartItem>
        {
            CartItem.Create(
                Company.Create("Test Co", "1234567890", "1234567890123", Account.Create("test@test.com", "hash", AccountType.Company)),
                batch,
                6)
        };

        _cartRepositoryMock
            .Setup(x => x.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cartItems);

        // Tariff
        var tariff = new OrderTariff
        {
            OrderTariffId = tariffId,
            Name = "Standard",
            MarkupPercent = 10m,
            FreeEmployeesPercent = 50m
        };
        _tariffRepositoryMock
            .Setup(x => x.GetByIdAsync(tariffId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tariff);

        // Employees
        var employees = new List<Employee>
        {
            Employee.Create(1, "John", "Doe", "Petrovich", new DateOnly(1990, 1, 1), null)
        };
        _employeeRepositoryMock
            .Setup(x => x.GetActiveWithTasksAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        // Production plan
        var productionPlan = new ProductionPlan(
            new DateOnly(2024, 3, 14),
            desiredDeliveryDate,
            new DateTime(2024, 3, 13, 9, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 3, 15, 18, 0, 0, DateTimeKind.Utc),
            2,
            employees);

        _productionCalculatorMock
            .Setup(x => x.CalculatePlan(cartItems, tariff, It.IsAny<DateTime>(), desiredDeliveryDate, employees))
            .Returns(productionPlan);

        // Proposed deferred date
        var deferredDate = new DateOnly(2024, 3, 20);
        _workScheduleCalculator
            .Setup(x => x.AddWorkDays(desiredDeliveryDate.ToDateTime(TimeOnly.MinValue), 3))
            .Returns(deferredDate.ToDateTime(TimeOnly.MinValue));

        // Ingredient check - INSUFFICIENT
        var deficits = new List<IngredientRequirement>
        {
            new(1, "Flour", 1000m, 500m, 500m),     // Need 1000, have 500
            new(2, "Apples", 2000m, 1000m, 1000m)   // Need 2000, have 1000
        };

        var ingredientCheck = new IngredientCheckResult(
            false, // Insufficient!
            new List<IngredientRequirement>(), // Empty requirements
            deficits);

        _ingredientServiceMock
            .Setup(x => x.CheckAsync(It.IsAny<IReadOnlyList<OrderItem>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientCheck);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.True(result.InsufficientIngredients);
        Assert.Null(result.OrderId);
        Assert.Equal(2, result.Deficits.Count);
        Assert.Contains(result.Deficits, d => d.IngredientId == 1 && d.RequiredQuantity == 1000m && d.AvailableQuantity == 500m);
        Assert.Contains(result.Deficits, d => d.IngredientId == 2 && d.RequiredQuantity == 2000m && d.AvailableQuantity == 1000m);
        Assert.Equal(deferredDate, result.ProposedDeferredDate);

        // Verify NO order creation
        _orderRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _orderItemRepositoryMock.Verify(x => x.CreateRangeAsync(It.IsAny<IEnumerable<OrderItem>>(), It.IsAny<CancellationToken>()), Times.Never);
        _ingredientServiceMock.Verify(x => x.ReserveForOrderAsync(It.IsAny<IReadOnlyList<OrderItem>>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        _employeeTaskRepositoryMock.Verify(x => x.BulkCreateAsync(It.IsAny<IEnumerable<EmployeeTask>>(), It.IsAny<CancellationToken>()), Times.Never);
        _cartRepositoryMock.Verify(x => x.ClearAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyCart_ThrowsException()
    {
        // Arrange
        var command = new CreateOrderCommand(new DateOnly(2024, 3, 15), 1);

        _cartRepositoryMock
            .Setup(x => x.GetByCompanyIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CartItem>());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
