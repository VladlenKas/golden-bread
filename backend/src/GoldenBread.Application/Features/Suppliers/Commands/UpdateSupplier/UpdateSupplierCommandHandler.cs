using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Suppliers.Commands.UpdateSupplier;

public sealed class UpdateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSupplierCommand, bool>
{
    public async Task<bool> Handle(UpdateSupplierCommand request, CancellationToken ct)
    {
        var dto = request.SupplierDto;
        var supplier = await supplierRepository.GetByIdAsync(dto.SupplierId, ct);

        if (supplier is null)
            return false;

        supplier.Update(dto.Name, dto.Email, dto.Phone, dto.Address);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}
