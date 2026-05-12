namespace GoldenBread.Application.Features.SupplierIngredient.Commands.DeleteSupplierIngredient;

public sealed record DeleteSupplierIngredientCommand(int SupplierIngredientId) : IRequest<bool>;