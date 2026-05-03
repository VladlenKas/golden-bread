using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.Suppliers.Dtos;

namespace GoldenBread.Application.Features.Suppliers.Queries.GetSupplierById;

public sealed class GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository)
    : IRequestHandler<GetSupplierByIdQuery, SupplierDto?>
{
    public async Task<SupplierDto?> Handle(GetSupplierByIdQuery query, CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(query.Id, ct);
        if (supplier == null)
            return null;

        return new SupplierDto(
            supplier.SupplierId,
            supplier.Name,
            supplier.Email,
            supplier.Phone,
            supplier.Address);
    }
}