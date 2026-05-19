using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;


namespace GoldenBread.Application.Features.Products.Commands.CreateProductWithDetails;

public sealed class CreateProductWithDetailsCommandHandler(
    IGoldenBreadContext context,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductWithDetailsCommand, int>
{
    public async Task<int> Handle(CreateProductWithDetailsCommand request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var p = request.Product;
            var product = new Product
            {
                Name = p.Name,
                Description = p.Description,
                CostPrice = p.CostPrice,
                Weight = p.Weight,
                ProductionTimeMinutes = p.ProductionTimeMinutes,
                ShelfLifeDays = p.ShelfLifeDays,
                StorageTempMin = p.StorageTempMin,
                StorageTempMax = p.StorageTempMax,
                CategoryId = p.CategoryId
            };

            await productRepository.AddAsync(product, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // Recipes
            foreach (var r in request.Recipes)
            {
                context.Recipes.Add(new Recipe
                {
                    ProductId = product.ProductId,
                    IngredientId = r.IngredientId,
                    Quantity = r.Quantity
                });
            }

            // Batches
            foreach (var b in request.Batches)
            {
                context.ProductBatches.Add(ProductBatch.Create(
                    product.ProductId,
                    b.QuantityUnits,
                    b.MarkupPercent));
            }

            // Images
            foreach (var img in request.ImagePaths)
            {
                context.ProductImages.Add(new ProductImage
                {
                    ProductId = product.ProductId,
                    ImagePath = img
                });
            }

            await unitOfWork.SaveChangesAsync(ct);
            await unitOfWork.CommitAsync(ct);

            return product.ProductId;
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
