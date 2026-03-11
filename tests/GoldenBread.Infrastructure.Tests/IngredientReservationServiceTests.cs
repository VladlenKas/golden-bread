//using GoldenBread.Application.Abstractions.Data;
//using GoldenBread.Application.Abstractions.Repositories;
//using GoldenBread.Domain.Entities;
//using GoldenBread.Domain.Enums;
//using GoldenBread.Infrastructure.Services;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Xunit;

//namespace GoldenBread.Infrastructure.Tests;

//public class IngredientReservationServiceTests
//{
//    private readonly Mock<IIngredientBatchRepository> _batchRepositoryMock;
//    private readonly Mock<IIngredientReservationRepository> _reservationRepositoryMock;
//    private readonly Mock<IGoldenBreadContext> _contextMock;
//    private readonly IngredientReservationService _service;

//    public IngredientReservationServiceTests()
//    {
//        _batchRepositoryMock = new Mock<IIngredientBatchRepository>();
//        _reservationRepositoryMock = new Mock<IIngredientReservationRepository>();
//        _contextMock = new Mock<IGoldenBreadContext>();

//        _service = new IngredientReservationService(
//            _batchRepositoryMock.Object,
//            _reservationRepositoryMock.Object,
//            _contextMock.Object);
//    }

//    [Fact]
//    public async Task CheckAsync_SufficientIngredients_ReturnsTrue()
//    {
//        // Arrange
//        var recipes = new List<Recipe>
//        {
//            new Recipe { IngredientId = 1, Ingredient = new Ingredient { IngredientId = 1, Name = "Flour" }, Quantity = 100 },
//            new Recipe { IngredientId = 2, Ingredient = new Ingredient { IngredientId = 2, Name = "Sugar" }, Quantity = 50 }
//        }.AsQueryable();

//        var mockRecipesDbSet = new Mock<DbSet<Recipe>>();
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.Provider).Returns(recipes.Provider);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.Expression).Returns(recipes.Expression);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.ElementType).Returns(recipes.ElementType);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.GetEnumerator()).Returns(recipes.GetEnumerator());

//        _contextMock.Setup(c => c.Recipes).Returns(mockRecipesDbSet.Object);

//        var orderItems = new List<OrderItem>
//        {
//            CreateOrderItem(1, 2, 2, new[] { (1, 100m), (2, 50m) }) // 2 batches, 2 per batch
//        };

//        var flourBatches = new List<IngredientBatch>
//        {
//            CreateIngredientBatch(1, 1, 500m, 
//                IngredientBatchStatus.Available, 
//                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
//                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)))
//        };
//        var sugarBatches = new List<IngredientBatch>
//        {
//            CreateIngredientBatch(2, 2, 300m, 
//                IngredientBatchStatus.Available, 
//                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
//                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)))
//        };

//        _batchRepositoryMock.Setup(b => b.GetAvailableForIngredientsAsync(1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(flourBatches);
//        _batchRepositoryMock.Setup(b => b.GetAvailableForIngredientsAsync(2, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(sugarBatches);

//        // Act
//        //var result = await _service.CheckAsync(orderItems);

//        //// Assert
//        //Assert.True(result.IsSufficient);
//        //Assert.Empty(result.Deficits);
//        //Assert.Equal(2, result.Requirements.Count);

//        //// Need: Flour 100*2*2=400, have 500 ✓
//        //// Need: Sugar 50*2*2=200, have 300 ✓
//        //var flourReq = result.Requirements.First(r => r.IngredientId == 1);
//        //Assert.Equal(400m, flourReq.RequiredQuantity);
//        //Assert.Equal(500m, flourReq.AvailableQuantity);
//    }

//    [Fact]
//    public async Task CheckAsync_InsufficientIngredients_ReturnsFalseWithDeficits()
//    {
//        // Arrange
//        var recipes = new List<Recipe>
//        {
//            new Recipe { IngredientId = 1, Ingredient = new Ingredient { IngredientId = 1, Name = "Flour" }, Quantity = 200 }
//        }.AsQueryable();

//        var mockRecipesDbSet = new Mock<DbSet<Recipe>>();
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.Provider).Returns(recipes.Provider);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.Expression).Returns(recipes.Expression);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.ElementType).Returns(recipes.ElementType);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.GetEnumerator()).Returns(recipes.GetEnumerator());

//        _contextMock.Setup(c => c.Recipes).Returns(mockRecipesDbSet.Object);

//        var orderItems = new List<OrderItem>
//        {
//            CreateOrderItem(1, 3, 2, new[] { (1, 200m) }) // 3 batches, 2 per batch = 6 items * 200 = 1200 needed
//        };

//        var flourBatches = new List<IngredientBatch>
//        {
//            CreateIngredientBatch(1, 1, 500m,
//                IngredientBatchStatus.Available,
//                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
//                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)))
//        };

//        _batchRepositoryMock.Setup(b => b.GetAvailableForIngredientsAsync(1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(flourBatches);

//        // Act
//        //var result = await _service.CheckAsync(orderItems);

//        //// Assert
//        //Assert.False(result.IsSufficient);
//        //Assert.Single(result.Deficits);
//        //Assert.Equal(1200m, result.Deficits[0].RequiredQuantity);
//        //Assert.Equal(500m, result.Deficits[0].AvailableQuantity);
//        //Assert.Equal(500m, result.Deficits[0].ReservedQuantity); // Available is reserved
//    }

//    [Fact]
//    public async Task CheckAsync_ExpiredBatches_Excluded()
//    {
//        // Arrange
//        var recipes = new List<Recipe>
//        {
//            new Recipe { IngredientId = 1, Ingredient = new Ingredient { IngredientId = 1, Name = "Flour" }, Quantity = 100 }
//        }.AsQueryable();

//        var mockRecipesDbSet = new Mock<DbSet<Recipe>>();
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.Provider).Returns(recipes.Provider);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.Expression).Returns(recipes.Expression);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.ElementType).Returns(recipes.ElementType);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.GetEnumerator()).Returns(recipes.GetEnumerator());

//        _contextMock.Setup(c => c.Recipes).Returns(mockRecipesDbSet.Object);

//        var orderItems = new List<OrderItem>
//        {
//            CreateOrderItem(1, 1, 1, new[] { (1, 100m) })
//        };

//        var flourBatches = new List<IngredientBatch>
//        {
//            CreateIngredientBatch(1, 1, 500m, 
//                IngredientBatchStatus.Available, 
//                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
//                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30))) // Expired!
//        };

//        _batchRepositoryMock.Setup(b => b.GetAvailableForIngredientsAsync(1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(flourBatches);

//        // Act
//        //var result = await _service.CheckAsync(orderItems);

//        // Assert
//        //Assert.False(result.IsSufficient);
//        //Assert.Equal(0m, result.Deficits[0].AvailableQuantity); // Expired excluded
//    }

//    [Fact]
//    public async Task ReserveForOrderAsync_CreatesReservations_FIFO()
//    {
//        // Arrange
//        var recipes = new List<Recipe>
//        {
//            new Recipe { ProductId = 1, IngredientId = 1, Quantity = 100 }
//        }.AsQueryable();

//        var mockRecipesDbSet = new Mock<DbSet<Recipe>>();
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.Provider).Returns(recipes.Provider);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.Expression).Returns(recipes.Expression);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.ElementType).Returns(recipes.ElementType);
//        mockRecipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.GetEnumerator()).Returns(recipes.GetEnumerator());

//        _contextMock.Setup(c => c.Recipes).Returns(mockRecipesDbSet.Object);

//        var orderItems = new List<OrderItem>
//        {
//            CreateOrderItem(1, 2, 1, new[] { (1, 100m) }) // 2 batches * 1 * 100 = 200 needed
//        };

//        // FIFO: older batch first
//        var batches = new List<IngredientBatch>
//        {
//            CreateIngredientBatch(1, 1, 80m, 
//                IngredientBatchStatus.Available, 
//                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 
//                new DateOnly(2024, 1, 1)),
//            CreateIngredientBatch(2, 1, 150m, 
//                IngredientBatchStatus.Available, 
//                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 
//                new DateOnly(2024, 2, 1))
//        };

//        _batchRepositoryMock.Setup(b => b.GetAvailableForIngredientsAsync(1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(batches);

//        List<IngredientReservation> capturedReservations = null;
//        _reservationRepositoryMock.Setup(r => r.CreateRangeAsync(It.IsAny<IEnumerable<IngredientReservation>>(), It.IsAny<CancellationToken>()))
//            .Callback<IEnumerable<IngredientReservation>, CancellationToken>((res, _) => capturedReservations = res.ToList());

//        // Act
//        await _service.ReserveForOrderAsync(orderItems, 123, false, CancellationToken.None);

//        // Assert
//        Assert.NotNull(capturedReservations);
//        Assert.Equal(2, capturedReservations.Count); // Two batches needed

//        // First batch: 80 (all available)
//        Assert.Equal(1, capturedReservations[0].IngredientBatchId);
//        Assert.Equal(80m, capturedReservations[0].ReservedQuantity);

//        // Second batch: 120 (remaining 200-80)
//        Assert.Equal(2, capturedReservations[1].IngredientBatchId);
//        Assert.Equal(120m, capturedReservations[1].ReservedQuantity);

//        Assert.All(capturedReservations, r => Assert.Equal(123, r.OrderId));
//    }

//    [Fact]
//    public async Task ConfirmReservationsAsync_ConfirmsActiveReservations()
//    {
//        // Arrange
//        var reservations = new List<IngredientReservation>
//        {
//            IngredientReservation.Create(123, 1, 100m),
//            IngredientReservation.Create(123, 2, 200m)
//        };

//        _reservationRepositoryMock.Setup(r => r.GetByOrderIdAsync(123, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(reservations);

//        // Act
//        await _service.ConfirmReservationsAsync(123);

//        // Assert
//        Assert.All(reservations, r => Assert.True(r.IsConfirmed));
//        _reservationRepositoryMock.Verify(r => r.UpdateRangeAsync(reservations, It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task CancelReservationsAsync_DeactivatesUnconfirmedReservations()
//    {
//        // Arrange
//        var reservations = new List<IngredientReservation>
//        {
//            IngredientReservation.Create(123, 1, 100m),
//            IngredientReservation.Create(123, 2, 200m)
//        };

//        _reservationRepositoryMock.Setup(r => r.GetByOrderIdAsync(123, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(reservations);

//        // Act
//        await _service.CancelReservationsAsync(123);

//        // Assert
//        Assert.All(reservations, r => Assert.False(r.IsActive));
//        _reservationRepositoryMock.Verify(r => r.UpdateRangeAsync(reservations, It.IsAny<CancellationToken>()), Times.Once);
//    }

//    // Helper methods
//    private OrderItem CreateOrderItem(int productId, int quantity, int unitsInBatch, (int ingredientId, decimal quantity)[] recipeData)
//    {
//        var product = Product.Create(productId, "Test", 60, 100m);
//        typeof(Product).GetProperty("ProductId")!.SetValue(product, productId);
//        typeof(Product).GetProperty("Recipes")!.SetValue(product, recipeData.Select(r => new Recipe
//        {
//            IngredientId = r.ingredientId,
//            Quantity = r.quantity
//        }).ToList());

//        var batch = ProductBatch.Create(1, product, unitsInBatch, 20);
//        typeof(ProductBatch).GetProperty("ProductBatchId")!.SetValue(batch, 1);
//        typeof(ProductBatch).GetProperty("Product")!.SetValue(batch, product);

//        return OrderItem.Create(
//            1, 1, batch, quantity, unitsInBatch, 100m);
//    }

//    private IngredientBatch CreateIngredientBatch(int id, int ingredientId, decimal remaining, IngredientBatchStatus status, DateOnly expiry, DateOnly? deliveryDate = null)
//    {
//        return IngredientBatch.Create(
//            id,
//            ingredientId,
//            0,
//            remaining,
//            deliveryDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
//            expiry,
//            status);
//    }
//}

