using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Exceptions;
using GoldenBread.Application.Features.CompanyOrder.Services;
using GoldenBread.Domain.Constants;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Infrastructure.Services;

public class IngredientReservationService(
    IIngredientReservationRepository reservationRepository,
    IGoldenBreadContext context) : IIngredientReservationService
{
    /// <summary>
    /// Проверка ингредиентов для формирования заказа
    /// (без резервирования продукции)
    /// </summary>
    /// <returns>True, если ингредиентов для формирования заказа хватает, иначе False</returns>
    public async Task<bool> CheckAsync(
        IReadOnlyList<OrderItem> orderItems,
        CancellationToken ct)
    {
        // 1. Загружаем все необходимые игредиенты для заказа
        var ingredientNeeds = await CalculateIngredientNeedsAsync(orderItems, ct);

        if (ingredientNeeds.Count == 0)
            return false;

        // 2. Проверяем достаточность ингредиентов для заказа
        var ingredientIds = ingredientNeeds.Select(n => n.IngredientId).ToList();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var stockByIngredient = await GetAvailableQuery(ingredientIds, today)
            .GroupBy(ib => ib.IngredientId)
            .Select(g => new
            {
                IngredientId = g.Key,
                Total = g.Sum(ib => ib.RemainingQuantity)
            })
            .ToDictionaryAsync(x => x.IngredientId, x => x.Total, ct);

        return ingredientNeeds.All(need =>
            stockByIngredient.GetValueOrDefault(need.IngredientId, 0) >= need.TotalNeeded);
    }

    /// <summary>
    /// Резервирует ингредиенты для заказа из доступных ингредиентов в партиях
    /// и создает запись в БД для IngredientReservation
    /// </summary>
    /// <param name="orderItems"></param>
    /// <param name="orderId"></param>
    /// <param name="ct"></param>
    /// <exception cref="InsufficientIngredientsException">Недостаточно ингредиентов в партиях</exception>
    public async Task ReserveForOrderAsync(
        IReadOnlyList<OrderItem> orderItems,
        int orderId,
        CancellationToken ct = default)
    {
        // 1. Загружаем все необходимые игредиенты для заказа
        var ingredientNeeds = await CalculateIngredientNeedsAsync(orderItems, ct);

        if (ingredientNeeds.Count == 0) 
            return;

        // 2. Получаем все доступные партии для резервирования ингредиентов
        var ingredientIds = ingredientNeeds.Select(n => n.IngredientId).ToList();
        var asOfDate = DateOnly.FromDateTime(DateTime.UtcNow + BakeryConstants.ReservationTimeout);

        var allBatches = await GetAvailableQuery(ingredientIds, asOfDate)
            .OrderBy(ib => ib.IngredientId)
            .ThenBy(ib => ib.DeliveryDate)
            .AsNoTracking()
            .ToListAsync(ct);

        // 3. Резервируем ингредиенты
        var reservations = new List<IngredientReservation>();

        foreach (var need in ingredientNeeds)
        {
            var batches = allBatches.Where(b => b.IngredientId == need.IngredientId);
            var remaining = need.TotalNeeded;

            foreach (var batch in batches)
            {
                if (remaining <= 0) 
                    break;

                var take = Math.Min(remaining, batch.RemainingQuantity);
                reservations.Add(
                    IngredientReservation.Create(
                        orderId, 
                        batch.IngredientBatchId, 
                        take));
                remaining -= take;
            }

            if (remaining > 0)
                throw new InvalidOperationException();
        }

        await reservationRepository.CreateRangeAsync(reservations, ct);
    }

    public async Task ConfirmReservationsAsync(int orderId, CancellationToken ct = default)
    {
        var reservations = await reservationRepository.GetByOrderIdAsync(orderId, ct);

        foreach (var reservation in reservations)
        {
            if (reservation.IsActive && !reservation.IsConfirmed)
                reservation.Confirm();
        }

        if (reservations.Any())
            await reservationRepository.UpdateRangeAsync(reservations, ct);
    }

    public async Task CancelReservationsAsync(int orderId, CancellationToken ct = default)
    {
        var reservations = await reservationRepository.GetByOrderIdAsync(orderId, ct);

        foreach (var reservation in reservations)
        {
            if (reservation.IsActive && !reservation.IsConfirmed)
                reservation.Deactivate(); 
        }

        if (reservations.Any())
            await reservationRepository.UpdateRangeAsync(reservations, ct);
    }

    private record IngredientNeed(int IngredientId, string Name, decimal TotalNeeded);

    private async Task<List<IngredientNeed>> CalculateIngredientNeedsAsync(
        IReadOnlyList<OrderItem> orderItems,
        CancellationToken ct)
    {
        // 1. Загружаем продукции из заказа
        var productIds = orderItems
            .Select(oi => oi.Batch.ProductId)
            .Distinct()
            .ToList();

        if (productIds.Count == 0)
            return new List<IngredientNeed>();

        // 2. Загружаем рецепты продукций по заказу
        var allRecipes = await context.Recipes
            .Where(r => productIds.Contains(r.ProductId))
            .Select(r => new
            {
                r.ProductId,
                r.IngredientId,
                r.Ingredient.Name,
                r.Quantity
            })
            .ToListAsync(ct);

        // 3. Загружаем потребности по ингредиентам
        var needs = orderItems
            .SelectMany(oi => allRecipes
                .Where(r => r.ProductId == oi.Batch.ProductId)
                .Select(r => new
                {
                    r.IngredientId,
                    r.Name,
                    Needed = r.Quantity * oi.QuantityPerBatch * oi.UnitsInBatch
                }))
            .GroupBy(x => x.IngredientId)
            .Select(g => new IngredientNeed(
                g.Key,
                g.First().Name,
                g.Sum(x => x.Needed)))
            .ToList();

        return needs;
    }

    private IQueryable<IngredientBatch> GetAvailableQuery(
        IReadOnlyList<int> ingredientIds,
        DateOnly asOfDate)
    {
        return context.IngredientBatches
            .Where(ib => ingredientIds.Contains(ib.IngredientId))
            .Where(ib => ib.Status == IngredientBatchStatus.Available)
            .Where(ib => ib.RemainingQuantity > 0)
            .Where(ib => ib.ExpiryDate > asOfDate);
    }
}
