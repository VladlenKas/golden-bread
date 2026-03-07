using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Application.Features.CompanyOrder.Services;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Infrastructure.Services;

public class IngredientReservationService(
    IIngredientBatchRepository batchRepository,
    IIngredientReservationRepository reservationRepository,
    IGoldenBreadContext context) : IIngredientReservationService
{
    public async Task<IngredientCheckResult> CheckAsync(
        IReadOnlyList<OrderItem> orderItems,
        CancellationToken cancellationToken = default)
    {
        // Собираем потребности по ингредиентам
        var requirements = new Dictionary<int, (string Name, decimal Required)>();

        foreach (var orderItem in orderItems)
        {
            // Загружаем рецепты для продукта
            var recipes = context.Recipes
                //.Where(r => r.ProductId == orderItem.Batch.ProductId)
                //.Include(r => r.Ingredient)
                .ToList();

            foreach (var recipe in recipes)
            {
                // Количество ингредиента = рецепт * количество партий * количество в партии
                var neededQuantity = recipe.Quantity * orderItem.QuantityPerBatch * orderItem.UnitsInBatch;

                if (requirements.ContainsKey(recipe.IngredientId))
                    requirements[recipe.IngredientId] = (
                        recipe.Ingredient.Name,
                        requirements[recipe.IngredientId].Required + neededQuantity);
                else
                    requirements[recipe.IngredientId] = (recipe.Ingredient.Name, neededQuantity);
            }
        }

        // Проверяем наличие на складе
        var resultRequirements = new List<IngredientRequirement>();
        var deficits = new List<IngredientRequirement>();
        bool isSufficient = true;

        foreach (var req in requirements)
        {
            var batches = await batchRepository.GetAvailableForIngredientAsync(req.Key, cancellationToken);
            var available = batches
                .Where(b => b.Status == IngredientBatchStatus.Available && b.ExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow))
                .Sum(b => b.RemainingQuantity);

            var requirement = new IngredientRequirement(
                req.Key,
                req.Value.Name,
                req.Value.Required,
                available,
                0); // Reserved пока 0

            resultRequirements.Add(requirement);

            if (available < req.Value.Required)
            {
                isSufficient = false;
                deficits.Add(requirement with { ReservedQuantity = available });
            }
        }

        return new IngredientCheckResult(isSufficient, resultRequirements, deficits);
    }

    public async Task ReserveForOrderAsync(
        IReadOnlyList<OrderItem> orderItems,
        int orderId,
        bool confirmed,
        CancellationToken cancellationToken = default)
    {
        var reservations = new List<IngredientReservation>();

        foreach (var orderItem in orderItems)
        {
            var recipes = context.Recipes
                .Where(r => r.ProductId == orderItem.Batch.ProductId)
                .ToList();

            foreach (var recipe in recipes)
            {
                var neededQuantity = recipe.Quantity * orderItem.QuantityPerBatch * orderItem.UnitsInBatch;

                // Берем самые старые партии (FIFO)
                var batches = await batchRepository.GetAvailableForIngredientAsync(recipe.IngredientId, cancellationToken);
                var availableBatches = batches
                    .Where(b => b.Status == IngredientBatchStatus.Available && b.ExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow))
                    .OrderBy(b => b.DeliveryDate)
                    .ToList();

                decimal remainingToReserve = neededQuantity;

                foreach (var batch in availableBatches)
                {
                    if (remainingToReserve <= 0) break;

                    var canReserve = Math.Min(remainingToReserve, batch.RemainingQuantity);
                    if (canReserve <= 0) continue;

                    reservations.Add(IngredientReservation.Create(
                        orderId,
                        batch.IngredientBatchId,
                        canReserve));

                    // Обновляем остаток (в реальности это должно быть в транзакции)
                    remainingToReserve -= canReserve;
                }
            }
        }

        await reservationRepository.CreateRangeAsync(reservations, cancellationToken);
    }

    public async Task ConfirmReservationsAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var reservations = await reservationRepository.GetByOrderIdAsync(orderId, cancellationToken);

        foreach (var reservation in reservations)
        {
            if (reservation.IsActive && !reservation.IsConfirmed)
                reservation.Confirm();
        }

        await reservationRepository.UpdateRangeAsync(reservations, cancellationToken);
    }

    public async Task CancelReservationsAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var reservations = await reservationRepository.GetByOrderIdAsync(orderId, cancellationToken);

        foreach (var reservation in reservations)
        {
            if (reservation.IsActive && !reservation.IsConfirmed)
                reservation.Deactivate(); // или Cancel()
        }

        await reservationRepository.UpdateRangeAsync(reservations, cancellationToken);
    }
}
