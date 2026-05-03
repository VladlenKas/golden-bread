using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Suppliers.Commands.DeleteSupplier;

public sealed class DeleteSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSupplierCommand, bool>
{
    public async Task<bool> Handle(DeleteSupplierCommand request, CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, ct);

        if (supplier is null)
            return false;

        supplier.DeletedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}