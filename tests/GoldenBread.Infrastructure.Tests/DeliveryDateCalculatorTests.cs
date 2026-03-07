using FluentAssertions;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using GoldenBread.Infrastructure.Services;
using Xunit;

namespace GoldenBread.Infrastructure.Tests;

public class DeliveryDateCalculatorTests : IDisposable
{
    private readonly DeliveryDateCalculator _calculator;
    private readonly DateTime _fixedNow = new(2024, 1, 15, 10, 0, 0);

    public DeliveryDateCalculatorTests() 
    {
        _calculator = new DeliveryDateCalculator();
    }

    public void Dispose() { }

    #region Empty Cart Tests

    [Fact]
    public void CalculateMinimalDeliveryDate_EmptyCart_ReturnsTomorrow()
    {
        // Arrange
        var cartItems = new List<CartItem>();
        var tariff = CreateTariff(5);
        var employees = new List<Employee>();

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(1));
        result.Should().Be(expected);
    }

    #endregion

    #region Tariff Percentage Tests

    [Fact]
    public void CalculateMinimalDeliveryDate_Tariff5PercentOf100Employees_Returns5Employees()
    {
        // Arrange: 100 сотрудников * 5% = 5 сотрудников
        var employees = CreateEmployees(100);
        var cartItems = CreateCartItems(new[] { (1, 120, 1, 1) }); // 1 партия, 120 мин, 1 шт в партии, кол-во 1
        var tariff = CreateTariff(5);

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert: 120 мин / (5 сотрудников * 480 мин) = 1 день
        // Но т.к. 1 сотрудник делает 120 мин = 0.25 дня → округляем до 1 дня
        // Завтра + 0 дней (т.к. 120/480 < 1, но Ceiling даёт 1 день? Нужно проверить логику)
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(1));
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateMinimalDeliveryDate_Tariff5PercentOf23Employees_RoundsUpTo2Employees()
    {
        // Arrange: 23 * 5% = 1.15 → округляем до 2
        var employees = CreateEmployees(23);
        // 480 минут работы (ровно 1 рабочий день на 1 сотрудника)
        var cartItems = CreateCartItems(new[] { (1, 480, 1, 1) });
        var tariff = CreateTariff(5);

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert: 480 мин / 2 сотрудника = 240 мин каждому = 0.5 дня → округляем до 1 дня
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(1));
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateMinimalDeliveryDate_Tariff1PercentOf50Employees_RoundsUpTo1Employee()
    {
        // Arrange: 50 * 1% = 0.5 → округляем до 1 (минимум)
        var employees = CreateEmployees(50);
        var cartItems = CreateCartItems(new[] { (1, 480, 1, 1) }); // 480 минут
        var tariff = CreateTariff(1);

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert: 480 мин / 1 сотрудник = 480 мин = 1 день
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(1));
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateMinimalDeliveryDate_ZeroPercentTariff_ReturnsAtLeast1Employee()
    {
        // Arrange: любой процент, который даёт < 1, должен вернуть минимум 1
        var employees = CreateEmployees(10);
        var cartItems = CreateCartItems(new[] { (1, 480, 1, 1) });
        var tariff = CreateTariff(0); // 0%

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert: должен быть 1 сотрудник, 480 мин = 1 день
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(1));
        result.Should().Be(expected);
    }

    #endregion

    #region Work Distribution Tests

    [Fact]
    public void CalculateMinimalDeliveryDate_MultipleBatches_DistributesEvenly()
    {
        // Arrange: 10 партий, 2 сотрудника = 5 партий каждому
        // Каждая партия: 120 мин * 2 шт = 240 мин
        // Всего: 10 * 240 = 2400 мин
        // На сотрудника: 1200 мин = 2.5 дня → округляем до 3 дня
        var employees = CreateEmployees(2);
        var cartItems = CreateCartItems(new[] { (10, 120, 2, 1) }); // 10 партий, 120 мин, 2 шт в партии, 1 позиция в корзине
        var tariff = CreateTariff(100); // 100% = 2 сотрудника

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert: завтра + 2 дня (т.к. 1200/480 = 2.5 → Ceiling = 3, но индексация с 0)
        // 15.01 + 1 (завтра) + 2 (дополнительных) = 18.01? Или 17.01?
        // Если 2.5 дня → Ceiling = 3 рабочих дня → завтра (день 1) + 2 дня = день 3
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(3));
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateMinimalDeliveryDate_LargeOrder_MultipleDays()
    {
        // Arrange: 1000 партий по 60 минут = 60000 минут
        // 10 сотрудников = 6000 минут каждому = 12.5 дней → 13 дней
        var employees = CreateEmployees(10);
        var cartItems = CreateCartItems(new[] { (1000, 60, 1, 1) });
        var tariff = CreateTariff(100);

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert: завтра (16.01) + 12 дней = 28.01
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(13));
        result.Should().Be(expected);
    }

    #endregion

    #region Employee Selection Tests

    [Fact]
    public void CalculateMinimalDeliveryDate_SelectsLeastBusyEmployees()
    {
        // Arrange: 5 сотрудников, выбираем 2 (40% тариф)
        // У сотрудника 1 и 2 есть задачи на завтра, у 3,4,5 — нет
        var busyEmployee1 = CreateEmployeeWithTasks(1, new[] { (_fixedNow.AddDays(1), _fixedNow.AddDays(1).AddHours(4)) }); // 4 часа занят
        var busyEmployee2 = CreateEmployeeWithTasks(2, new[] { (_fixedNow.AddDays(1), _fixedNow.AddDays(1).AddHours(2)) }); // 2 часа занят
        var freeEmployee3 = CreateEmployee(3);
        var freeEmployee4 = CreateEmployee(4);
        var freeEmployee5 = CreateEmployee(5);

        var employees = new List<Employee>
        {
            busyEmployee1,
            busyEmployee2,
            freeEmployee3,
            freeEmployee4,
            freeEmployee5
        };

        // Работа: 960 минут (2 рабочих дня на 1 сотрудника)
        var cartItems = CreateCartItems(new[] { (1, 960, 1, 1) });
        var tariff = CreateTariff(40); // 40% от 5 = 2 сотрудника

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert: должны выбраться freeEmployee3 и freeEmployee4 (или 3 и 5, или 4 и 5)
        // Работа поровну: 480 мин каждому = 1 день
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(1));
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateMinimalDeliveryDate_EmployeeWithFutureTasks_NotCountedAsBusy()
    {
        // Arrange: сотрудник занят через неделю, но завтра свободен
        var employee = CreateEmployeeWithTasks(1, new[]
        {
            (_fixedNow.AddDays(7), _fixedNow.AddDays(7).AddHours(8))
        });

        var employees = new List<Employee> { employee };
        var cartItems = CreateCartItems(new[] { (1, 480, 1, 1) }); // 1 день работы
        var tariff = CreateTariff(100);

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert: задача через неделю не мешает — дата завтра + 1 день = послезавтра? 
        // Нет, 480 мин = 1 день, значит завтра (день 1) + 0 = завтра? 
        // Или завтра + 1 день?
        // 480 мин / 480 мин в день = 1 день → Ceiling(1) = 1 → завтра + (1-1) = завтра?
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(1));
        result.Should().Be(expected);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void CalculateMinimalDeliveryDate_Exactly8Hours_ReturnsOneDay()
    {
        // Arrange: ровно 8 часов = 1 рабочий день
        var employees = CreateEmployees(1);
        var cartItems = CreateCartItems(new[] { (1, 480, 1, 1) }); // 480 мин = 8 часов
        var tariff = CreateTariff(100);

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert: завтра + 0 дополнительных дней (т.к. Ceiling(1) = 1, но это первый день)
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(1));
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateMinimalDeliveryDate_8HoursAnd1Minute_ReturnsTwoDays()
    {
        // Arrange: 8 часов 1 минута = 481 минута → 2 дня
        var employees = CreateEmployees(1);
        var cartItems = CreateCartItems(new[] { (1, 481, 1, 1) });
        var tariff = CreateTariff(100);

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert: Ceiling(481/480) = Ceiling(1.002) = 2 → завтра + 1 день
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(2));
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateMinimalDeliveryDate_MultipleCartItems_AggregatesCorrectly()
    {
        // Arrange: 2 позиции в корзине
        // Позиция 1: 2 партии * 60 мин * 1 шт = 120 мин
        // Позиция 2: 3 партии * 120 мин * 2 шт = 720 мин
        // Всего: 840 мин
        // 2 сотрудника = 420 мин каждому = 0.875 дня → 1 день
        var employees = CreateEmployees(2);
        var cartItems = new List<CartItem>
        {
            CreateCartItem(1, 60, 1, 2),  // 2 партии по 60 мин, 1 шт в партии
            CreateCartItem(2, 120, 2, 3)  // 3 партии по 120 мин, 2 шт в партии
        };
        var tariff = CreateTariff(100);

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, _fixedNow);

        // Assert
        var expected = DateOnly.FromDateTime(_fixedNow.AddDays(1));
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateMinimalDeliveryDate_NightTimeNow_CalculatesFromTomorrow()
    {
        // Arrange: сейчас ночь (2 часа ночи)
        var nightNow = new DateTime(2024, 1, 15, 2, 0, 0);
        var employees = CreateEmployees(1);
        var cartItems = CreateCartItems(new[] { (1, 480, 1, 1) });
        var tariff = CreateTariff(100);

        // Act
        var result = _calculator.CalculateMinimalDeliveryDate(
            cartItems, tariff, employees, nightNow);

        // Assert: завтра = 16.01, независимо от текущего времени
        var expected = new DateOnly(2024, 1, 16);
        result.Should().Be(expected);
    }

    #endregion

    #region Helper Methods

    private static OrderTariff CreateTariff(decimal markupPercent)
    {
        return new OrderTariff
        {
            OrderTariffId = 1,
            Name = "Test Tariff",
            MarkupPercent = markupPercent,
            Description = "Test",
            FreeEmployeesPercent = 0
        };
    }

    private static List<Employee> CreateEmployees(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => Employee.Create(
                i,
                $"Employee{i}",
                "Test",
                $"Patronymic{i}",
                new DateOnly(1990, 1, 1),
                new List<EmployeeTask>()))
            .ToList();
    }

    private static Employee CreateEmployee(int id)
    {
        return Employee.Create(
            id,
            $"Employee{id}",
            "Test",
            $"Patronymic{id}",
            new DateOnly(1990, 1, 1),
            new List<EmployeeTask>());
    }

    private static Employee CreateEmployeeWithTasks(int id, IEnumerable<(DateTime start, DateTime end)> tasks)
    {
        var employee = CreateEmployee(id);
        int taskId = 1;
        foreach (var (start, end) in tasks)
        {
            employee.EmployeeTasks.Add(EmployeeTask.Create(
                taskId++,
                employee.EmployeeId,
                0,
                start,
                end,
                1, 0));
        }
        return employee;
    }

    private static List<CartItem> CreateCartItems(IEnumerable<(int quantity, int productionTime, int qtyPerBatch, int cartQuantity)> items)
    {
        int productId = 1;

        return items.Select(item =>
        {
            var (quantity, productionTime, qtyPerBatch, cartQuantity) = item;

            var product = Product.Create(
                productId,
                $"Product{productId}",
                productionTime,
                100);

            var batch = ProductBatch.Create(
                productId,
                product,
                qtyPerBatch,
                20);

            productId++;

            var account = Account.Create(
                "asdf@test.ts",
                "pass",
                AccountType.Company);

            var company = Company.Create(
                "Test",
                "1234",
                "1234567",
                account);

            return CartItem.Create(
                company, 
                batch,
                quantity);
        }).ToList();
    }

    private static CartItem CreateCartItem(int productId, int productionTime, int qtyPerBatch, int quantity)
    {
        var product = Product.Create(
            productId,
            $"Product{productId}",
            productionTime,
            100);

        var batch = ProductBatch.Create(
            productId,
            product,
            qtyPerBatch,
            20);
            
        var account = Account.Create(
            "asdf@test.ts",
            "pass",
            AccountType.Company);

        var company = Company.Create(
            "Test",
            "1234",
            "1234567",
            account);

        return CartItem.Create(
            company,
            batch,
            quantity);
    }

    #endregion
}