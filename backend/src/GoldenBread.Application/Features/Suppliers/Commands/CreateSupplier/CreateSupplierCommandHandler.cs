using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Suppliers.Commands.CreateSupplier;

public sealed class CreateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSupplierCommand, int>
{
    public async Task<int> Handle(CreateSupplierCommand request, CancellationToken ct)
    {
        var dto = request.SupplierDto;
        var supplier = Supplier.Create(0, dto.Name, dto.Email, dto.Phone, dto.Address);

        await supplierRepository.AddAsync(supplier, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return supplier.SupplierId;
    }
}