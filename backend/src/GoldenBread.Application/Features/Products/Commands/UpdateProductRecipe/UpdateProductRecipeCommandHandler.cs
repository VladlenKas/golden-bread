using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Products.Commands.UpdateProductRecipe;

public sealed class UpdateProductRecipeCommandHandler(
    IGoldenBreadContext context,
    IProductRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductRecipeCommand, bool>
{
    public async Task<bool> Handle(UpdateProductRecipeCommand request, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(request.ProductId, ct);
        if (product is null) return false;

        var existing = product.Recipes.ToList();
        var incoming = request.Recipes;

        // Delete missing
        foreach (var e in existing)
        {
            if (!incoming.Any(i => i.RecipeId == e.RecipeId))
                context.Recipes.Remove(e);
        }

        // Update existing
        foreach (var i in incoming.Where(i => i.RecipeId.HasValue))
        {
            var e = existing.FirstOrDefault(x => x.RecipeId == i.RecipeId);
            if (e != null)
            {
                e.IngredientId = i.IngredientId;
                e.Quantity = i.Quantity;
            }
        }

        // Add new
        foreach (var i in incoming.Where(i => !i.RecipeId.HasValue || i.RecipeId == 0))
        {
            context.Recipes.Add(new Recipe
            {
                ProductId = request.ProductId,
                IngredientId = i.IngredientId,
                Quantity = i.Quantity
            });
        }

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}