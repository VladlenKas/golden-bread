//using GoldenBread.Application.Abstractions.Data;
//using GoldenBread.Application.Abstractions.Services;
//using GoldenBread.Application.Features.Cart.Dtos;
//using GoldenBread.Application.Features.Cart.Services;
//using GoldenBread.Domain.Entities;
//using GoldenBread.Domain.Enums;
//using Microsoft.Extensions.Logging;
//namespace GoldenBread.Application.Features.Cart.Commands.CreateOrder;

//public class CreateOrderCommandHandler :
//    IRequestHandler<CreateOrderCommand, OrderCreationResponse>
//{
//    private readonly IGoldenBreadContext _context;
//    private readonly ICurrentAccountContext _accountContext;
//    private readonly IDeliveryDateCalculator _deliveryCalculator;
//    private readonly IEmployeeTaskService _taskService;
//    private readonly IUnitConversionService _unitConverter;
//    private readonly ILogger<CreateOrderCommandHandler> _logger;

//    public CreateOrderCommandHandler(
//        IGoldenBreadContext context,
//        ICurrentAccountContext accountContext,
//        IDeliveryDateCalculator deliveryCalculator,
//        IEmployeeTaskService taskService,
//        IUnitConversionService unitConverter,
//        ILogger<CreateOrderCommandHandler> logger)
//    {
//        _context = context;
//        _accountContext = accountContext;
//        _deliveryCalculator = deliveryCalculator;
//        _taskService = taskService;
//        _unitConverter = unitConverter;
//        _logger = logger;
//    }

//    public async Task<OrderCreationResponse> Handle(
//        CreateOrderCommand request,
//        CancellationToken cancellationToken)
//    {
//        var account = await _accountContext.GetAccountAsync(cancellationToken);
//        var companyId = account.Company.CompanyId;
//        var now = DateTime.UtcNow;

//        // 1. Загружаем выбранные партии с полными данными
//        var batchIds = request.CartItems.Select(ci => ci.ProductBatchId).ToList();

//        var batches = await _context.ProductBatches
//            .Where(b => batchIds.Contains(b.ProductBatchId))
//            .Include(b => b.Product)
//                .ThenInclude(p => p.Recipes)
//                    .ThenInclude(r => r.Ingredient)
//            .ToListAsync(cancellationToken);

//        // Проверяем, что все партии найдены
//        if (batches.Count != batchIds.Count)
//        {
//            var missingIds = batchIds.Except(batches.Select(b => b.ProductBatchId));
//            throw new InvalidOperationException($"Product batches not found: {string.Join(", ", missingIds)}");
//        }

//        // 2. Формируем "виртуальные" CartItem для расчётов
//        var virtualCartItems = request.CartItems
//            .Select(ci => new
//            {
//                Batch = batches.First(b => b.ProductBatchId == ci.ProductBatchId),
//                Quantity = ci.QuantityInCart
//            })
//            .ToList();

//        // 3. Рассчитываем требуемые ингредиенты (с конвертацией единиц в базовые)
//        var requiredIngredients = CalculateRequiredIngredients(virtualCartItems);

//        if (!requiredIngredients.Any())
//        {
//            throw new InvalidOperationException("No ingredients required for selected products");
//        }

//        // 4. Проверяем доступность ингредиентов (FIFO, только Available, не просроченные)
//        var checkResult = await CheckIngredientAvailabilityAsync(
//            requiredIngredients,
//            request.SelectedDeliveryDate,
//            cancellationToken);

//        // 5. Если не хватает — возвращаем информацию о недостатке (БЕЗ создания заказа!)
//        if (!checkResult.IsSufficient)
//        {
//            return new OrderCreationResponse
//            {
//                Success = false,
//                InsufficientIngredients = true,
//                MissingIngredients = checkResult.MissingIngredients
//            };
//        }

//        // 6. Всё хватает — создаём заказ в транзакции
//        await using var transaction = await _context.Database
//            .BeginTransactionAsync(cancellationToken);

//        try
//        {
//            // 6.1. Создаём заказ
//            var order = Order.Create(
//                companyId,
//                request.SelectedTariffId,
//                OrderStatus.InProgress,
//                request.SelectedDeliveryDate);

//            _context.Orders.Add(order);
//            await _context.SaveChangesAsync(cancellationToken); // Получаем OrderId

//            // 6.2. Списываем ингредиенты (уменьшаем RemainingQuantity)
//            await DeductIngredientsAsync(checkResult.Allocations, cancellationToken);

//            // 6.3. Создаём OrderItems
//            var orderItems = new List<OrderItem>();
//            foreach (var item in virtualCartItems)
//            {
//                var orderItem = OrderItem.Create(
//                    order.OrderId,
//                    item.Batch.ProductBatchId,
//                    item.Quantity,
//                    item.Quantity * item.Batch.QuantityPerBatch,
//                    item.Batch.QuantityPerBatch,
//                    item.Batch.UnitPrice);

//                _context.OrderItems.Add(orderItem);
//                orderItems.Add(orderItem);
//            }

//            await _context.SaveChangesAsync(cancellationToken); // Получаем OrderItemIds

//            // 6.4. Создаём задачи сотрудникам
//            // Преобразуем virtualCartItems в формат, понятный сервису
//            var cartItemsForTaskService = virtualCartItems.Select(v => new CartItem
//            {
//                Batch = v.Batch,
//                Quantity = v.Quantity,
//                CompanyId = companyId
//            }).ToList();

//            await _taskService.CreateTasksForOrderAsync(
//                order,
//                cartItemsForTaskService,
//                cancellationToken);

//            // 6.5. Очищаем корзину компании (удаляем CartItem-ы с этими партиями)
//            await ClearCartAsync(companyId, batchIds, cancellationToken);

//            await _context.SaveChangesAsync(cancellationToken);
//            await transaction.CommitAsync(cancellationToken);

//            _logger.LogInformation(
//                "Order {OrderId} created successfully for company {CompanyId}",
//                order.OrderId, companyId);

//            return new OrderCreationResponse
//            {
//                Success = true,
//                OrderId = order.OrderId,
//                Status = OrderStatus.InProgress,
//                DeliveryDate = request.SelectedDeliveryDate,
//                IsDelayed = false
//            };
//        }
//        catch (Exception ex)
//        {
//            await transaction.RollbackAsync(cancellationToken);
//            _logger.LogError(ex, "Failed to create order for company {CompanyId}", companyId);
//            throw;
//        }
//    }

//    // Расчёт требуемых ингредиентов с конвертацией в базовые единицы
//    private Dictionary<int, IngredientRequirement> CalculateRequiredIngredients(
//        List<dynamic> virtualCartItems)
//    {
//        var requirements = new Dictionary<int, IngredientRequirement>();

//        foreach (var item in virtualCartItems)
//        {
//            var totalUnits = item.Quantity * item.Batch.QuantityPerBatch;
//            var recipes = item.Batch.Product.Recipes;

//            foreach (var recipe in recipes)
//            {
//                // Конвертируем количество из рецепта в базовую единицу
//                var baseQuantity = _unitConverter.ToBaseUnit(
//                    recipe.Quantity * totalUnits,
//                    recipe.Ingredient.Unit);

//                if (requirements.TryGetValue(recipe.IngredientId, out var existing))
//                {
//                    existing.BaseQuantity += baseQuantity;
//                }
//                else
//                {
//                    requirements[recipe.IngredientId] = new IngredientRequirement
//                    {
//                        IngredientId = recipe.IngredientId,
//                        IngredientName = recipe.Ingredient.Name,
//                        BaseQuantity = baseQuantity,
//                        BaseUnit = GetBaseUnit(recipe.Ingredient.Unit),
//                        OriginalUnit = recipe.Ingredient.Unit
//                    };
//                }
//            }
//        }

//        return requirements;
//    }

//    // Проверка доступности ингредиентов (FIFO)
//    private async Task<IngredientCheckResult> CheckIngredientAvailabilityAsync(
//        Dictionary<int, IngredientRequirement> requirements,
//        DateOnly deliveryDate,
//        CancellationToken cancellationToken)
//    {
//        var allocations = new List<IngredientAllocation>();
//        var missing = new List<MissingIngredientDto>();

//        foreach (var req in requirements.Values)
//        {
//            // Ищем доступные батчи: Available, не просроченные (с запасом до даты доставки)
//            var availableBatches = await _context.IngredientBatches
//                .Where(b => b.IngredientId == req.IngredientId
//                    && b.Status == IngredientBatchStatus.Available
//                    && b.ExpiryDate > deliveryDate) // Должен храниться до даты доставки
//                .OrderBy(b => b.ExpiryDate) // FIFO: сначала те, что скоро испортятся
//                .ThenBy(b => b.DeliveryDate)
//                .ToListAsync(cancellationToken);

//            var totalAvailable = availableBatches.Sum(b => b.RemainingQuantity);

//            if (totalAvailable < req.BaseQuantity)
//            {
//                missing.Add(new MissingIngredientDto
//                {
//                    IngredientId = req.IngredientId,
//                    IngredientName = req.IngredientName,
//                    RequiredQuantity = req.BaseQuantity,
//                    AvailableQuantity = totalAvailable,
//                    Unit = req.BaseUnit
//                });
//                continue;
//            }

//            // Формируем распределение по батчам
//            var remaining = req.BaseQuantity;
//            foreach (var batch in availableBatches)
//            {
//                if (remaining <= 0) break;

//                var take = Math.Min(batch.RemainingQuantity, remaining);

//                allocations.Add(new IngredientAllocation
//                {
//                    IngredientBatchId = batch.IngredientBatchId,
//                    IngredientId = req.IngredientId,
//                    BaseQuantity = take,
//                    Batch = batch
//                });

//                remaining -= take;
//            }
//        }

//        return new IngredientCheckResult
//        {
//            IsSufficient = !missing.Any(),
//            Allocations = allocations,
//            MissingIngredients = missing
//        };
//    }

//    // Списание ингредиентов
//    private async Task DeductIngredientsAsync(
//        List<IngredientAllocation> allocations,
//        CancellationToken cancellationToken)
//    {
//        foreach (var allocation in allocations)
//        {
//            var batch = allocation.Batch;
//            batch.RemainingQuantity -= allocation.BaseQuantity;

//            if (batch.RemainingQuantity <= 0)
//            {
//                batch.Status = IngredientBatchStatus.OutOfStock;
//            }

//            // Не вызываем SaveChanges здесь — будет в транзакции выше
//        }

//        await Task.CompletedTask;
//    }

//    // Очистка корзины
//    private async Task ClearCartAsync(
//        int companyId,
//        List<int> batchIds,
//        CancellationToken cancellationToken)
//    {
//        var cartItemsToRemove = await _context.CartItems
//            .Where(ci => ci.CompanyId == companyId && batchIds.Contains(ci.BatchId))
//            .ToListAsync(cancellationToken);

//        _context.CartItems.RemoveRange(cartItemsToRemove);
//    }

//    private static IngredientUnit GetBaseUnit(IngredientUnit unit) => unit switch
//    {
//        IngredientUnit.Kg => IngredientUnit.G,
//        IngredientUnit.L => IngredientUnit.Ml,
//        _ => unit
//    };
//}

//// Вспомогательные классы
//public class IngredientRequirement
//{
//    public int IngredientId { get; set; }
//    public string IngredientName { get; set; } = null!;
//    public decimal BaseQuantity { get; set; }
//    public IngredientUnit BaseUnit { get; set; }
//    public IngredientUnit OriginalUnit { get; set; }
//}

//public class IngredientAllocation
//{
//    public int IngredientBatchId { get; set; }
//    public int IngredientId { get; set; }
//    public decimal BaseQuantity { get; set; }
//    public IngredientBatch Batch { get; set; } = null!;
//}

//public class IngredientCheckResult
//{
//    public bool IsSufficient { get; set; }
//    public List<IngredientAllocation> Allocations { get; set; } = new();
//    public List<MissingIngredientDto> MissingIngredients { get; set; } = new();
//}
