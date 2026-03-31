using FluentAssertions;
using GoldenBread.Domain.Constants;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.Services;
using Moq;
using System.Diagnostics;
using Xunit;

namespace GoldenBread.Infrastructure.Tests;

public class EmployeeTaskDistributorTests
{
    private readonly Mock<IWorkScheduleService> _scheduleMock;
    private readonly EmployeeTaskDistributor _distributor;

    public EmployeeTaskDistributorTests()
    {
        _scheduleMock = new Mock<IWorkScheduleService>();
        _distributor = new EmployeeTaskDistributor(_scheduleMock.Object);
    }

    #region Тестовые данные и хелперы

    private static Product CreateProduct(int productionTimeMinutes = 30)
    {
        return Product.Create("Test Product", productionTimeMinutes);
    }

    private static ProductBatch CreateBatch(int batchId, Product product)
    {
        // Используем рефлексию или создаём через фабрику с минимальными данными
        // Для тестов нам нужен Batch с Product, используем Create с productId, затем устанавливаем Product через приватный сеттер или reflection
        var batch = ProductBatch.Create(product.ProductId, 10, 20);

        // Устанавливаем Product через reflection, так как в фабрике нет параметра
        typeof(ProductBatch).GetProperty(nameof(ProductBatch.Product))!
            .SetValue(batch, product);

        // Устанавливаем BatchId через reflection (обычно из БД)
        typeof(ProductBatch).GetProperty(nameof(ProductBatch.ProductBatchId))!
            .SetValue(batch, batchId);

        return batch;
    }

    private static OrderItem CreateOrderItem(int orderItemId, int quantity, int unitsPerBatch, ProductBatch batch)
    {
        var orderItem = OrderItem.Create(
            orderId: 1,
            batchId: batch.ProductBatchId,
            quantity: quantity,
            unitsPerBatch: unitsPerBatch,
            unitPrice: 100m);

        // Устанавливаем OrderItemId и Batch через reflection
        typeof(OrderItem).GetProperty(nameof(OrderItem.OrderItemId))!
            .SetValue(orderItem, orderItemId);

        typeof(OrderItem).GetProperty(nameof(OrderItem.Batch))!
            .SetValue(orderItem, batch);

        return orderItem;
    }

    private static Employee CreateEmployee(int employeeId, string firstname = "Test", string lastname = "Employee")
    {
        return Employee.Create(
            employeeId: employeeId,
            firstname: firstname,
            lastname: lastname,
            patronymic: null,
            birthday: new DateOnly(1990, 1, 1));
    }

    private void SetupDefaultScheduleBehavior(DateTime baseTime)
    {
        _scheduleMock.Setup(s => s.GetNextAvailableTime(It.IsAny<Employee>(), It.IsAny<DateTime>()))
            .Returns((Employee e, DateTime d) => d);

        _scheduleMock.Setup(s => s.SnapToWorkTime(It.IsAny<DateTime>()))
            .Returns((DateTime d) => d);

        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));
    }

    #endregion

    #region Сценарий 1: Когда сотрудники свободные

    [Fact]
    public void Distribute_WhenAllEmployeesFree_ShouldDistributeEvenly()
    {
        // Arrange: Понедельник 9:00 UTC
        var monday = new DateTime(2026, 3, 30, 9, 0, 0, DateTimeKind.Utc);

        var product = CreateProduct(productionTimeMinutes: 30);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 10, unitsPerBatch: 10, batch);

        var employees = new List<Employee>
        {
            CreateEmployee(1, "Иван"),
            CreateEmployee(2, "Петр"),
            CreateEmployee(3, "Сидор")
        };

        var availableFrom = new Dictionary<int, DateTime>();

        _scheduleMock.Setup(s => s.GetNextAvailableTime(It.IsAny<Employee>(), It.IsAny<DateTime>()))
        .Returns(monday);

        _scheduleMock.Setup(s => s.SnapToWorkTime(It.IsAny<DateTime>()))
            .Returns((DateTime d) => d);

        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 100m,
            monday);

        // Assert
        result.Should().HaveCount(3);
        result.Sum(r => r.AssignedQuantity).Should().Be(10);

        // Распределение: 4, 3, 3
        result.Should().Contain(r => r.EmployeeId == 1 && r.AssignedQuantity == 4);
        result.Should().Contain(r => r.EmployeeId == 2 && r.AssignedQuantity == 3);
        result.Should().Contain(r => r.EmployeeId == 3 && r.AssignedQuantity == 3);

        // Все начинают в 9:00 (SnapToWorkTime прилипает к началу рабочего дня)
        result.All(r => r.StartTime == monday).Should().BeTrue();

        // EndTime расчёт:
        // Сотрудник 1: 4 × 30 = 120 мин. 9:00 + 120 мин = 11:00 (до обеда)
        result.First(r => r.EmployeeId == 1).EndTime.Should().Be(monday.AddHours(2));

        // Сотрудник 2: 3 × 30 = 90 мин. 9:00 + 90 мин = 10:30
        result.First(r => r.EmployeeId == 2).EndTime.Should().Be(monday.AddMinutes(90));

        // Сотрудник 3: 3 × 30 = 90 мин. 9:00 + 90 мин = 10:30
        result.First(r => r.EmployeeId == 3).EndTime.Should().Be(monday.AddMinutes(90));

        // availableFrom обновлено на EndTime каждого
        availableFrom[1].Should().Be(monday.AddHours(2));
        availableFrom[2].Should().Be(monday.AddMinutes(90));
        availableFrom[3].Should().Be(monday.AddMinutes(90));
    }

    [Fact]
    public void Distribute_WhenAllEmployeesFree_WithSingleEmployee_ShouldAssignAll()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 29, 9, 0, 0, DateTimeKind.Utc);
        var product = CreateProduct(productionTimeMinutes: 20);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 5, unitsPerBatch: 5, batch);

        var employees = new List<Employee> { CreateEmployee(1) };
        var availableFrom = new Dictionary<int, DateTime>();

        _scheduleMock.Setup(s => s.GetNextAvailableTime(It.IsAny<Employee>(), It.IsAny<DateTime>()))
        .Returns(baseTime);

        _scheduleMock.Setup(s => s.SnapToWorkTime(It.IsAny<DateTime>()))
            .Returns((DateTime d) => d); // ← Identity function!

        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 100m,
            baseTime);

        // Assert
        result.Should().HaveCount(1);
        result[0].AssignedQuantity.Should().Be(5);
        result[0].StartTime.Should().Be(baseTime);
        result[0].EndTime.Should().Be(baseTime.AddMinutes(100));
    }

    #endregion

    #region Сценарий 2: Когда у сотрудников частично заняты дни

    [Fact]
    public void Distribute_WhenEmployeesPartiallyBusy_ShouldRespectAvailableFrom()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 29, 9, 0, 0);
        var product = CreateProduct(productionTimeMinutes: 60);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 6, unitsPerBatch: 6, batch);

        var employees = new List<Employee>
        {
            CreateEmployee(1, "Иван"),
            CreateEmployee(2, "Петр")
        };

        // Сотрудник 1 занят до 12:00, сотрудник 2 занят до 14:00
        var availableFrom = new Dictionary<int, DateTime>
        {
            [1] = baseTime.AddHours(3),  // 12:00
            [2] = baseTime.AddHours(5)   // 14:00
        };

        _scheduleMock.Setup(s => s.SnapToWorkTime(It.IsAny<DateTime>()))
            .Returns((DateTime d) => d);
        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        result.Should().HaveCount(2);
        result.Sum(r => r.AssignedQuantity).Should().Be(6);

        // 6 единиц на 2 = 3 и 3
        var emp1Result = result.First(r => r.EmployeeId == 1);
        var emp2Result = result.First(r => r.EmployeeId == 2);

        emp1Result.AssignedQuantity.Should().Be(3);
        emp2Result.AssignedQuantity.Should().Be(3);

        // Проверяем что учитывается availableFrom
        emp1Result.StartTime.Should().Be(baseTime.AddHours(3)); // 12:00
        emp2Result.StartTime.Should().Be(baseTime.AddHours(5)); // 14:00

        emp1Result.EndTime.Should().Be(baseTime.AddHours(3).AddMinutes(180)); // 12:00 + 3*60
        emp2Result.EndTime.Should().Be(baseTime.AddHours(5).AddMinutes(180)); // 14:00 + 3*60

        // Обновленные времена доступности
        availableFrom[1].Should().Be(emp1Result.EndTime);
        availableFrom[2].Should().Be(emp2Result.EndTime);
    }

    [Fact]
    public void Distribute_WhenSomeEmployeesNotInDictionary_ShouldCalculateAvailability()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 29, 9, 0, 0);
        var calculatedTime = baseTime.AddHours(2); // Рассчитанное время для нового сотрудника

        var product = CreateProduct(productionTimeMinutes: 30);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 4, unitsPerBatch: 4, batch);

        var employees = new List<Employee>
        {
            CreateEmployee(1),
            CreateEmployee(2) // Нет в словаре availableFrom
        };

        var availableFrom = new Dictionary<int, DateTime>
        {
            [1] = baseTime // Сотрудник 1 свободен сейчас
        };

        _scheduleMock.Setup(s => s.GetNextAvailableTime(employees[1], It.IsAny<DateTime>()))
            .Returns(calculatedTime);
        _scheduleMock.Setup(s => s.SnapToWorkTime(It.IsAny<DateTime>()))
            .Returns((DateTime d) => d);
        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        result.Should().HaveCount(2);

        var emp1Result = result.First(r => r.EmployeeId == 1);
        var emp2Result = result.First(r => r.EmployeeId == 2);

        emp1Result.StartTime.Should().Be(baseTime);
        emp2Result.StartTime.Should().Be(calculatedTime); // Рассчитанное время

        // 4 единицы = 2 и 2
        emp1Result.AssignedQuantity.Should().Be(2);
        emp2Result.AssignedQuantity.Should().Be(2);
    }

    #endregion

    #region Сценарий 3: Один полностью занят первый день, другие частично/полностью заняты последние дни
    [Fact]
    public void Distribute_WhenOneFullyBusyFirstDay_OthersPartiallyBusyLastDays_ShouldHandleComplexAvailability()
    {
        // Arrange
        var day1Start = new DateTime(2026, 3, 29, 9, 0, 0);
        var day2Start = day1Start.AddDays(1);

        var product = CreateProduct(productionTimeMinutes: 120); // 2 часа на единицу
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 8, unitsPerBatch: 8, batch);

        var employees = new List<Employee>
        {
            CreateEmployee(1, "Занятой"),    // Полностью занят первый день (до 11:00 дня 2)
            CreateEmployee(2, "Частично"),  // Частично занят (до 13:00 дня 1)
            CreateEmployee(3, "Свободный")  // Свободен с 09:00 дня 1
        };

        // Сотрудник 1 занят весь первый день (до следующего дня)
        // Сотрудник 2 занят половину первого дня
        // Сотрудник 3 свободен
        var availableFrom = new Dictionary<int, DateTime>
        {
            [1] = day2Start.AddHours(2),      // Доступен с 11:00 дня 2 (самый поздний)
            [2] = day1Start.AddHours(4),      // Доступен с 13:00 дня 1
            [3] = day1Start                   // Доступен с 09:00 дня 1 (самый ранний)
        };

        _scheduleMock.Setup(s => s.SnapToWorkTime(It.IsAny<DateTime>()))
            .Returns((DateTime d) => d);
        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        result.Should().HaveCount(3);
        result.Sum(r => r.AssignedQuantity).Should().Be(8);

        // После сортировки по AvailableFrom: Employee 3 (09:00) → Employee 2 (13:00) → Employee 1 (11:00 дня 2)
        // 8 на 3 = 3, 3, 2 (первые 2 получают +1)
        var emp1Result = result.First(r => r.EmployeeId == 1);
        var emp2Result = result.First(r => r.EmployeeId == 2);
        var emp3Result = result.First(r => r.EmployeeId == 3);

        // Самые свободные получают больше работы (логично для скорости выполнения заказа)
        emp3Result.AssignedQuantity.Should().Be(3); // Самый свободный
        emp2Result.AssignedQuantity.Should().Be(3); // Средний
        emp1Result.AssignedQuantity.Should().Be(2); // Самый занятый

        // Проверяем времена начала с учётом занятости
        emp3Result.StartTime.Should().Be(day1Start);               // 09:00 дня 1 (свободен)
        emp2Result.StartTime.Should().Be(day1Start.AddHours(4));   // 13:00 дня 1
        emp1Result.StartTime.Should().Be(day2Start.AddHours(2));   // 11:00 дня 2 (занят весь день 1)

        // Проверяем длительность (количество * 120 минут)
        emp3Result.EndTime.Should().Be(emp3Result.StartTime.AddMinutes(360)); // 3 * 120
        emp2Result.EndTime.Should().Be(emp2Result.StartTime.AddMinutes(360)); // 3 * 120
        emp1Result.EndTime.Should().Be(emp1Result.StartTime.AddMinutes(240)); // 2 * 120

        // Проверяем обновление словаря
        availableFrom[1].Should().Be(emp1Result.EndTime);
        availableFrom[2].Should().Be(emp2Result.EndTime);
        availableFrom[3].Should().Be(emp3Result.EndTime);
    }

    [Fact]
    public void Distribute_WhenFreeEmployeesPercentLimitsCount_ShouldUseOnlyAvailableEmployees()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 29, 9, 0, 0);
        var product = CreateProduct(productionTimeMinutes: 30);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 10, unitsPerBatch: 10, batch);

        var employees = new List<Employee>
        {
            CreateEmployee(1),
            CreateEmployee(2),
            CreateEmployee(3),
            CreateEmployee(4),
            CreateEmployee(5)
        };

        // Все заняты до разного времени
        var availableFrom = new Dictionary<int, DateTime>
        {
            [1] = baseTime,                    // Свободен
            [2] = baseTime.AddHours(2),        // Занят до 11:00
            [3] = baseTime.AddHours(4),        // Занят до 13:00
            [4] = baseTime.AddHours(6),        // Занят до 15:00
            [5] = baseTime.AddHours(8)         // Занят до 17:00
        };

        SetupDefaultScheduleBehavior(baseTime);

        // Act - используем только 50% свободных (округление вверх: 5 * 0.5 = 2.5 -> 3)
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 50m,
            DateTime.UtcNow);

        // Assert
        result.Should().HaveCount(3); // Только 3 сотрудника из 5 (50% округлено вверх)
        result.Sum(r => r.AssignedQuantity).Should().Be(10);

        // 10 на 3 = 4, 3, 3
        result.Should().Contain(r => r.AssignedQuantity == 4);
        result.Should().Contain(r => r.AssignedQuantity == 3);

        // Должны быть выбраны первые 3 сотрудника (Take(3))
        result.Select(r => r.EmployeeId).Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    #endregion

    #region Граничные случаи и валидация

    [Fact]
    public void Distribute_WhenNoEmployees_ShouldReturnEmpty()
    {
        // Arrange
        var product = CreateProduct();
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 5, unitsPerBatch: 5, batch);

        // Act
        var result = _distributor.Distribute(
            orderItem,
            new List<Employee>(),
            new Dictionary<int, DateTime>(),
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Distribute_WhenQuantityIsZero_ShouldReturnEmpty()
    {
        // Arrange
        var product = CreateProduct();
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 0, unitsPerBatch: 10, batch);

        var employees = new List<Employee> { CreateEmployee(1) };

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            new Dictionary<int, DateTime>(),
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Distribute_WhenBatchProductIsNull_ShouldReturnEmpty()
    {
        // Arrange
        var orderItem = OrderItem.Create(1, 1, 5, 5, 100m);
        // Batch не установлен (null)

        var employees = new List<Employee> { CreateEmployee(1) };

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            new Dictionary<int, DateTime>(),
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Distribute_WhenUnitsPerBatchIsZero_ShouldReturnEmpty()
    {
        // Arrange
        var product = CreateProduct();
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 5, unitsPerBatch: 0, batch); // Некорректное значение

        var employees = new List<Employee> { CreateEmployee(1) };

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            new Dictionary<int, DateTime>(),
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Distribute_WhenProductionTimeIsZero_ShouldReturnEmpty()
    {
        // Arrange
        var product = CreateProduct(productionTimeMinutes: 0);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 5, unitsPerBatch: 5, batch);

        var employees = new List<Employee> { CreateEmployee(1) };

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            new Dictionary<int, DateTime>(),
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Distribute_WhenQuantityLessThanEmployeeCount_ShouldLimitEmployees()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 29, 9, 0, 0);
        var product = CreateProduct(productionTimeMinutes: 30);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 2, unitsPerBatch: 2, batch); // Только 2 единицы

        var employees = new List<Employee>
        {
            CreateEmployee(1),
            CreateEmployee(2),
            CreateEmployee(3),
            CreateEmployee(4),
            CreateEmployee(5)
        };

        SetupDefaultScheduleBehavior(baseTime);

        // Act - хотим 100% от 5 = 5 сотрудников, но товаров только 2
        var result = _distributor.Distribute(
            orderItem,
            employees,
            new Dictionary<int, DateTime>(),
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        result.Should().HaveCount(2); // Не больше, чем количество единиц
        result.Sum(r => r.AssignedQuantity).Should().Be(2);

        // 2 на 2 = 1 и 1
        result.All(r => r.AssignedQuantity == 1).Should().BeTrue();
    }

    [Fact]
    public void Distribute_WhenFreeEmployeesPercentIsLessThanOne_ShouldTreatAsOne()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 29, 9, 0, 0);
        var product = CreateProduct(productionTimeMinutes: 30);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 10, unitsPerBatch: 10, batch);

        var employees = new List<Employee>
        {
            CreateEmployee(1),
            CreateEmployee(2),
            CreateEmployee(3)
        };

        SetupDefaultScheduleBehavior(baseTime);

        // Act - 0.5% меньше 1, должно стать 1 сотрудник (Math.Max(1, ...))
        var result = _distributor.Distribute(
            orderItem,
            employees,
            new Dictionary<int, DateTime>(),
            freeEmployeesPercent: 0.5m,
            DateTime.UtcNow);

        // Assert
        result.Should().HaveCount(1); // Минимум 1 сотрудник
        result[0].AssignedQuantity.Should().Be(10); // Всё на одного
    }

    [Fact]
    public void Distribute_WhenFreeEmployeesPercentMoreThan100_ShouldClampTo100()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 29, 9, 0, 0);
        var product = CreateProduct(productionTimeMinutes: 30);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 10, unitsPerBatch: 10, batch);

        var employees = new List<Employee>
        {
            CreateEmployee(1),
            CreateEmployee(2)
        };

        SetupDefaultScheduleBehavior(baseTime);

        // Act - 150% должно стать 100%
        var result = _distributor.Distribute(
            orderItem,
            employees,
            new Dictionary<int, DateTime>(),
            freeEmployeesPercent: 150m,
            DateTime.UtcNow);

        // Assert
        result.Should().HaveCount(2); // 100% от 2 = 2 сотрудника
    }

    #endregion

    #region Проверка интеграции со scheduleCalculator

    [Fact]
    public void Distribute_ShouldCallSnapToWorkTime_ForStartAndEnd()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 29, 9, 0, 0);
        var snappedTime = new DateTime(2026, 3, 29, 10, 0, 0);

        var product = CreateProduct(productionTimeMinutes: 30);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 2, unitsPerBatch: 2, batch);

        var employees = new List<Employee> { CreateEmployee(1) };
        var availableFrom = new Dictionary<int, DateTime>();

        _scheduleMock.Setup(s => s.SnapToWorkTime(It.IsAny<DateTime>()))
            .Returns(snappedTime);
        _scheduleMock.Setup(s => s.AddWorkMinutes(snappedTime, 60))
            .Returns(snappedTime.AddMinutes(60));

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        _scheduleMock.Verify(s => s.SnapToWorkTime(It.IsAny<DateTime>()), Times.Exactly(2)); // Для start и nextAvailable
        result[0].StartTime.Should().Be(snappedTime);
    }

    [Fact]
    public void Distribute_ShouldUpdateEmployeeAvailableFrom_Correctly()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 29, 9, 0, 0);
        var endTime = baseTime.AddMinutes(120);
        var nextAvailable = endTime.AddMinutes(30);

        var product = CreateProduct(productionTimeMinutes: 60);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 2, unitsPerBatch: 2, batch);

        var employees = new List<Employee> { CreateEmployee(1) };
        var availableFrom = new Dictionary<int, DateTime>();

        // ИСПРАВЛЕНИЕ: Настраиваем GetNextAvailableTime для сотрудника, которого нет в словаре
        _scheduleMock.Setup(s => s.GetNextAvailableTime(employees[0], It.IsAny<DateTime>()))
            .Returns(baseTime);

        // ИСПРАВЛЕНИЕ: SnapToWorkTime должен возвращать переданное значение (identity)
        _scheduleMock.Setup(s => s.SnapToWorkTime(It.IsAny<DateTime>()))
            .Returns((DateTime d) => d);

        _scheduleMock.Setup(s => s.AddWorkMinutes(baseTime, 120)).Returns(endTime);
        // SnapToWorkTime(endTime) уже настроен выше через It.IsAny<DateTime>()

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 100m,
            DateTime.UtcNow);

        // Assert
        availableFrom.Should().ContainKey(1);
        availableFrom[1].Should().Be(endTime); // SnapToWorkTime(endTime) возвращает endTime
    }

    #endregion

    #region Дополнительные тесты для реальной проверки бизнес-логики
    [Fact]
    public void Distribute_WhenTariffPercentResultsInFraction_ShouldRoundUp()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 30, 9, 0, 0, DateTimeKind.Utc);
        var product = CreateProduct(productionTimeMinutes: 20);
        var batch = CreateBatch(1, product);
        var orderItem = CreateOrderItem(1, quantity: 50, unitsPerBatch: 1, batch);

        // 7 сотрудников, тариф 30% = 2.1 → должно округлиться до 3
        var employees = Enumerable.Range(1, 7)
            .Select(id => CreateEmployee(id))
            .ToList();

        var availableFrom = employees.ToDictionary(
            e => e.EmployeeId,
            e => baseTime.AddHours(e.EmployeeId)); // Разное время

        SetupDefaultScheduleBehavior(baseTime);

        // Act
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 30m, // 7 * 0.3 = 2.1 → 3 сотрудника
            currentTime: baseTime);

        // Assert
        result.Should().HaveCount(3, because: "30% от 7 = 2.1, округляем вверх до 3");

        // Проверяем распределение 50 единиц на 3: 17, 17, 16
        result.Sum(r => r.AssignedQuantity).Should().Be(50);
        result.Count(r => r.AssignedQuantity == 17).Should().Be(2);
        result.Count(r => r.AssignedQuantity == 16).Should().Be(1);
    }

    [Fact]
    public void Distribute_WhenUnitsLessThanTariffEmployees_ShouldLimitByUnitsAndSelectMostAvailable()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 30, 9, 0, 0, DateTimeKind.Utc);

        var product = CreateProduct(productionTimeMinutes: 30);
        var batch = CreateBatch(1, product);

        // Всего 3 единицы, но тариф позволяет взять 50% от 10 = 5 сотрудников
        // Должны выбраться только 3 самых свободных (не больше чем единиц)
        var orderItem = CreateOrderItem(1, quantity: 3, unitsPerBatch: 1, batch);

        var employees = Enumerable.Range(1, 10)
            .Select(id => CreateEmployee(id, $"Employee{id}"))
            .ToList();

        // Занятость: чем выше ID, тем занятей
        var availableFrom = employees.ToDictionary(
            e => e.EmployeeId,
            e => baseTime.AddHours(e.EmployeeId - 1)); // Emp1: сейчас, Emp2: +1 час, ..., Emp10: +9 часов

        SetupDefaultScheduleBehavior(baseTime);

        // Act: Тариф 50% = 5 сотрудников, но единиц только 3
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 50m,
            currentTime: baseTime);

        // Assert
        result.Should().HaveCount(3, because: "количество сотрудников не может превышать количество единиц продукции");

        // Должны быть выбраны только 1, 2, 3 (самые свободные)
        result.Select(r => r.EmployeeId).Should().BeEquivalentTo(new[] { 1, 2, 3 });

        // Распределение: 3 единицы на 3 сотрудника = 1 каждому
        result.All(r => r.AssignedQuantity == 1).Should().BeTrue();
    }

    [Fact]
    public void Distribute_ComplexScenario_WithTenEmployeesAndFortyPercentTariff_ShouldSelectMostAvailableAndDistributeCorrectly()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 30, 9, 0, 0, DateTimeKind.Utc); // Понедельник 9:00

        // Продукт: 1 час на единицу (60 мин)
        var product = CreateProduct(productionTimeMinutes: 60);
        var batch = CreateBatch(1, product);

        // Большой заказ: 100 единиц (100 часов работы суммарно)
        var orderItem = CreateOrderItem(1, quantity: 100, unitsPerBatch: 1, batch);

        // 10 сотрудников с разной загруженностью:
        // - 1-2: свободны сейчас (09:00)
        // - 3-4: заняты до обеда (свободны с 12:00)
        // - 5-6: заняты до вечера (свободны с 15:00) 
        // - 7-8: заняты весь день (свободны завтра в 09:00)
        // - 9-10: заняты два дня (свободны послезавтра в 09:00)
        var employees = Enumerable.Range(1, 10)
            .Select(id => CreateEmployee(id, $"Employee{id}"))
            .ToList();

        var availableFrom = new Dictionary<int, DateTime>
        {
            [1] = baseTime,                              // 09:00 сегодня (самый свободный)
            [2] = baseTime.AddMinutes(30),               // 09:30 сегодня 
            [3] = baseTime.AddHours(3),                  // 12:00 сегодня
            [4] = baseTime.AddHours(3).AddMinutes(30),   // 12:30 сегодня
            [5] = baseTime.AddHours(6),                  // 15:00 сегодня
            [6] = baseTime.AddHours(7),                  // 16:00 сегодня
            [7] = baseTime.AddDays(1),                   // 09:00 завтра
            [8] = baseTime.AddDays(1).AddHours(2),       // 11:00 завтра
            [9] = baseTime.AddDays(2),                   // 09:00 послезавтра
            [10] = baseTime.AddDays(2).AddHours(4)       // 13:00 послезавтра (самый занятый)
        };

        _scheduleMock.Setup(s => s.SnapToWorkTime(It.IsAny<DateTime>()))
            .Returns((DateTime d) => d);
        _scheduleMock.Setup(s => s.AddWorkMinutes(It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns((DateTime start, int minutes) => start.AddMinutes(minutes));

        // Для сотрудников, которых нет в словаре (на всякий случай)
        _scheduleMock.Setup(s => s.GetNextAvailableTime(It.IsAny<Employee>(), It.IsAny<DateTime>()))
            .Returns(baseTime);

        // Act: Тариф 40% от 10 = 4 сотрудника (Math.Ceiling(10 * 0.4) = 4)
        var result = _distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 40m,
            currentTime: baseTime);

        // Assert

        // 1. Должны быть выбраны ровно 4 сотрудника (40% от 10)
        result.Should().HaveCount(4);

        // 2. Это должны быть 4 самых свободных (ID: 1, 2, 3, 4)
        var selectedIds = result.Select(r => r.EmployeeId).OrderBy(id => id).ToList();
        selectedIds.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 },
            because: "тариф 40% должен отобрать только 4 самых свободных сотрудников из 10");

        // 3. Проверяем, что самые занятые (5-10) не попали в выборку
        result.Should().NotContain(r => r.EmployeeId >= 5 && r.EmployeeId <= 10);

        // 4. Распределение 100 единиц на 4 сотрудника = 25 каждому (ровно делится)
        result.Sum(r => r.AssignedQuantity).Should().Be(100);
        result.All(r => r.AssignedQuantity == 25).Should().BeTrue(
            because: "100 единиц на 4 сотрудника должны распределиться поровну (25 каждому)");

        // 5. Проверяем времена начала (должны соответствовать их availableFrom)
        var emp1 = result.First(r => r.EmployeeId == 1);
        var emp2 = result.First(r => r.EmployeeId == 2);
        var emp3 = result.First(r => r.EmployeeId == 3);
        var emp4 = result.First(r => r.EmployeeId == 4);

        emp1.StartTime.Should().Be(baseTime);                    // 09:00
        emp2.StartTime.Should().Be(baseTime.AddMinutes(30));     // 09:30
        emp3.StartTime.Should().Be(baseTime.AddHours(3));        // 12:00
        emp4.StartTime.Should().Be(baseTime.AddHours(3).AddMinutes(30)); // 12:30

        // 6. Проверяем длительность (25 единиц * 60 минут = 1500 минут = 25 часов)
        // Это размажется на несколько дней через AddWorkMinutes, но здесь проверяем математику
        emp1.EndTime.Should().Be(baseTime.AddMinutes(25 * 60));
        emp2.EndTime.Should().Be(baseTime.AddMinutes(30).AddMinutes(25 * 60));
        emp3.EndTime.Should().Be(baseTime.AddHours(3).AddMinutes(25 * 60));
        emp4.EndTime.Should().Be(baseTime.AddHours(3).AddMinutes(30).AddMinutes(25 * 60));

        // 7. Проверяем, что availableFrom обновился для выбранных сотрудников
        availableFrom[1].Should().Be(emp1.EndTime);
        availableFrom[2].Should().Be(emp2.EndTime);
        availableFrom[3].Should().Be(emp3.EndTime);
        availableFrom[4].Should().Be(emp4.EndTime);

        // 8. Проверяем, что остальные сотрудники не затронуты в словаре (или остались как были)
        availableFrom[5].Should().Be(baseTime.AddHours(6)); // Не изменился
        availableFrom[10].Should().Be(baseTime.AddDays(2).AddHours(4)); // Не изменился
    }

    [Fact]
    public void Distribute_WithRealScheduleService_ShouldRespectLunchBreakAndWorkDayBoundaries()
    {
        // Arrange - используем РЕАЛЬНЫЙ сервис, а не мок!
        var scheduleService = new WorkScheduleService();
        var distributor = new EmployeeTaskDistributor(scheduleService);

        // Создаем продукт: 90 минут на единицу (1.5 часа)
        var product = CreateProduct(productionTimeMinutes: 90);
        var batch = CreateBatch(1, product);

        // Заказ: 2 единицы = 3 часа работы
        var orderItem = CreateOrderItem(1, quantity: 2, unitsPerBatch: 1, batch);

        // Сотрудник, который занят до 11:00 (до обеда)
        var employee = CreateEmployee(1, "Иван");
        employee.EmployeeTasks = new List<EmployeeTask>
    {
        // Задача с 9:00 до 11:00 (занят 2 часа утра)
        EmployeeTask.Create(
            employeeId: 1,
            orderItemId: 999,
            startTime: new DateTime(2026, 3, 30, 9, 0, 0, DateTimeKind.Utc),
            endTime: new DateTime(2026, 3, 30, 11, 0, 0, DateTimeKind.Utc),
            assignedQuantity: 1,
            completedQuantity: 1)
    };

        // Текущее время: 10:00 (сотрудник еще занят)
        var currentTime = new DateTime(2026, 3, 30, 10, 0, 0, DateTimeKind.Utc);

        var availableFrom = new Dictionary<int, DateTime>();

        // Act
        var result = distributor.Distribute(
            orderItem,
            new List<Employee> { employee },
            availableFrom,
            freeEmployeesPercent: 100m,
            currentTime: currentTime);

        // Assert
        result.Should().HaveCount(1);
        var assignment = result[0];

        // StartTime должен быть 11:00 (когда освободился), но SnapToWorkTime приведет к 11:00
        assignment.StartTime.Should().Be(new DateTime(2026, 3, 30, 11, 0, 0, DateTimeKind.Utc));

        // Важно: 2 единицы × 90 мин = 180 минут (3 часа)
        // Начало в 11:00, обед с 12:00 до 13:00
        // 11:00-12:00 = 60 минут
        // Обед (пропускаем)
        // 13:00-15:00 = 120 минут
        // Итого EndTime должен быть 15:00 (не 14:00!)
        assignment.EndTime.Should().Be(new DateTime(2026, 3, 30, 15, 0, 0, DateTimeKind.Utc));

        // Проверяем, что словарь обновлен с учетом обеда
        availableFrom[1].Should().Be(assignment.EndTime);
    }

    [Fact]
    public void Distribute_WhenAllEmployeesMeetDeadline_ShouldSucceed()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 30, 9, 0, 0, DateTimeKind.Utc); // Понедельник

        var scheduleService = new WorkScheduleService();

        var testDate = new DateTime(2026, 3, 31, 16, 0, 0, DateTimeKind.Utc);
        var snapped = scheduleService.SnapToWorkTime(testDate);

        var distributor = new EmployeeTaskDistributor(scheduleService);

        // Продукт: 2 часа на единицу
        var product = CreateProduct(productionTimeMinutes: 120);
        var batch = CreateBatch(1, product);

        // Заказ: 6 единиц (12 часов работы), доставка в пятницу (3-4 дня вперёд — точно успеем)
        var orderEndDate = new DateOnly(2026, 4, 3); // Пятница
        var order = Order.Create(1, 1, OrderStatus.Awaiting, orderEndDate);
        var orderItem = CreateOrderItem(1, quantity: 6, unitsPerBatch: 1, batch);
        typeof(OrderItem).GetProperty(nameof(OrderItem.Order))!.SetValue(orderItem, order);

        // 3 сотрудника, заняты до 12:00 (полдня свободны по 3 часа = 180 мин)
        // Каждый возьмёт 2 единицы = 4 часа (240 мин)
        // Начало: 12:00, конец: 16:00 (до конца рабочего дня 17:00 — успевают)
        var employees = new List<Employee>
        {
            CreateEmployee(1, "Иван"),
            CreateEmployee(2, "Петр"),
            CreateEmployee(3, "Сидор")
        };

        var availableFrom = new Dictionary<int, DateTime>
        {
            [1] = baseTime.AddHours(3), // 12:00
            [2] = baseTime.AddHours(3),
            [3] = baseTime.AddHours(3)
        };

        // Act
        var result = distributor.Distribute(
            orderItem,
            employees,
            availableFrom,
            freeEmployeesPercent: 100m, // Все 3 сотрудника (30% от 3 = 1, но min = 3 из-за количества единиц)
            currentTime: baseTime);

        // Assert
        result.Should().HaveCount(3);
        result.Sum(r => r.AssignedQuantity).Should().Be(6);

        // Все закончили до 17:00 пятницы (3 апреля 2026)
        var deadline = new DateTime(2026, 4, 3, 17, 0, 0, DateTimeKind.Utc);
        result.All(r => r.EndTime <= deadline).Should().BeTrue();
    }

    [Fact]
    public void Distribute_WhenEmployeesMissDeadline_ShouldThrowException()
    {
        var baseTime = new DateTime(2026, 3, 30, 9, 0, 0, DateTimeKind.Utc);
        var scheduleService = new WorkScheduleService();
        var distributor = new EmployeeTaskDistributor(scheduleService);

        var product = CreateProduct(productionTimeMinutes: 180); // 3 часа
        var batch = CreateBatch(1, product);

        // Дедлайн: завтра 17:00
        var orderEndDate = new DateOnly(2026, 3, 31);
        var order = Order.Create(1, 1, OrderStatus.Awaiting, orderEndDate);
        var orderItem = CreateOrderItem(1, quantity: 4, unitsPerBatch: 1, batch);
        typeof(OrderItem).GetProperty(nameof(OrderItem.Order))!.SetValue(orderItem, order);

        // 2 сотрудника, свободны с 16:00 (1 час сегодня + 8 завтра = 9 часов < 12 часов работы)
        var employees = new List<Employee> { CreateEmployee(1), CreateEmployee(2) };
        var availableFrom = new Dictionary<int, DateTime>
        {
            [1] = baseTime.AddHours(7), // 16:00
            [2] = baseTime.AddHours(7)
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            distributor.Distribute(orderItem, employees, availableFrom, freeEmployeesPercent: 100m, currentTime: baseTime));

        // Проверяем наличие ключевых слов, а не формат даты (избегаем проблем с локалью)
        ex.Message.Should().Contain("deadline");
        ex.Message.Should().Contain("Employee 1"); // Или 2, кто первый в списке
        ex.Message.Should().Contain("Consider upgrading tariff");
    }

    [Fact]
    public void Distribute_WhenEmployeesFinishExactlyAtDeadline_ShouldSucceed()
    {
        // Arrange
        var baseTime = new DateTime(2026, 3, 30, 9, 0, 0, DateTimeKind.Utc);

        var scheduleService = new WorkScheduleService();
        var distributor = new EmployeeTaskDistributor(scheduleService);

        // Продукт: 1 час
        var product = CreateProduct(productionTimeMinutes: 60);
        var batch = CreateBatch(1, product);

        // Заказ на завтра 17:00 (конец рабочего дня)
        var orderEndDate = new DateOnly(2026, 3, 31);
        var order = Order.Create(1, 1, OrderStatus.Awaiting, orderEndDate);
        var orderItem = CreateOrderItem(1, quantity: 1, unitsPerBatch: 1, batch);
        typeof(OrderItem).GetProperty(nameof(OrderItem.Order))!.SetValue(orderItem, order);

        // Сотрудник свободен завтра в 16:00 (закончит в 17:00 — ровно дедлайн)
        var employee = CreateEmployee(1);
        var availableFrom = new Dictionary<int, DateTime>
        {
            [1] = new DateTime(2026, 3, 31, 16, 0, 0, DateTimeKind.Utc) // Завтра 16:00
        };

        // Act
        var result = distributor.Distribute(
            orderItem,
            new List<Employee> { employee },
            availableFrom,
            freeEmployeesPercent: 100m,
            currentTime: baseTime);

        // Assert
        result[0].EndTime.Should().Be(new DateTime(2026, 3, 31, 17, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Distribute_StressTest_WithLargeDataset_ShouldCompleteInReasonableTime()
    {
        // Arrange
        const int employeeCount = 100;
        const int orderItemCount = 50;
        const int unitsPerItem = 20; // 20 единиц

        var scheduleService = new WorkScheduleService();
        var distributor = new EmployeeTaskDistributor(scheduleService);

        var product = CreateProduct(productionTimeMinutes: 30);
        var batch = CreateBatch(1, product);

        var random = new Random(42);
        var employees = new List<Employee>();
        var availableFrom = new Dictionary<int, DateTime>();

        var baseTime = new DateTime(2026, 3, 30, 9, 0, 0, DateTimeKind.Utc);

        for (int i = 1; i <= employeeCount; i++)
        {
            var emp = CreateEmployee(i, $"Employee{i}");
            employees.Add(emp);
            availableFrom[i] = baseTime.AddMinutes(random.Next(0, 480));
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var allResults = new List<IReadOnlyList<EmployeeTaskAssignment>>();

        // Act
        for (int i = 0; i < orderItemCount; i++)
        {
            var orderItem = CreateOrderItem(i, quantity: unitsPerItem, unitsPerBatch: 1, batch);

            var result = distributor.Distribute(
                orderItem,
                employees,
                availableFrom,
                freeEmployeesPercent: 30m, // 30% от 100 = 30, но ограничено 20 единицами
                currentTime: baseTime);

            allResults.Add(result);
        }

        stopwatch.Stop();

        // Assert

        // 1. Производительность
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000);

        // 2. Все позиции распределены полностью
        allResults.Should().HaveCount(orderItemCount);
        allResults.Sum(r => r.Sum(a => a.AssignedQuantity)).Should().Be(orderItemCount * unitsPerItem);

        // 3. Количество сотрудников = min(тариф%, количество единиц)
        // 30% от 100 = 30, но единиц только 20, поэтому 20 сотрудников
        var expectedEmployeeCount = Math.Min(
            (int)Math.Ceiling(employeeCount * 0.3m), // 30
            unitsPerItem // 20
        ); // = 20

        allResults.All(r => r.Count == expectedEmployeeCount).Should().BeTrue(
            $"каждый OrderItem должен быть распределен на {expectedEmployeeCount} сотрудников " +
            $"(min(30% от 100, {unitsPerItem} единиц))");

        // 4. Распределение поровну (20 единиц на 20 человек = 1 каждому)
        allResults.SelectMany(r => r).All(a => a.AssignedQuantity == 1).Should().BeTrue();

        // 5. Словарь обновился
        var updatedCount = availableFrom.Count(kv => kv.Value > baseTime.AddHours(2));
        updatedCount.Should().BeGreaterThan(0);
    }
    #endregion

    #region Дебаг
    [Fact]
    public void Debug_Constants()
    {
        Debug.WriteLine($"MaxPlanningDays: {WorkScheduleConstants.MaxPlanningDays}");
        Debug.WriteLine($"WorkStartHour: {WorkScheduleConstants.WorkStartHour}");
        Debug.WriteLine($"WorkEndHour: {WorkScheduleConstants.WorkEndHour}");

        // Должно быть: 90, 8, 17
        WorkScheduleConstants.MaxPlanningDays.Should().BeGreaterThan(0);
    }
    #endregion
}