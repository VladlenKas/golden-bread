namespace GoldenBread.Application.Features.Suppliers.Commands.DeleteSupplier;

public sealed record DeleteSupplierCommand(int SupplierId) : IRequest<bool>;