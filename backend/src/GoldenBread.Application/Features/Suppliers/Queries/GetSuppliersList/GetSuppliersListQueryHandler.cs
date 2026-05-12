using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.Suppliers.Dtos;

namespace GoldenBread.Application.Features.Suppliers.Queries.GetSuppliersList;

public sealed class GetSuppliersListQueryHandler(ISupplierRepository supplierRepository)
    : IRequestHandler<GetSuppliersListQuery, SuppliersListResponse>
{
    public async Task<SuppliersListResponse> Handle(GetSuppliersListQuery query, CancellationToken ct)
    {
        var suppliers = await supplierRepository.GetAllAsync(ct);

        var list = suppliers.Select(s => new SupplierListItem(
            s.SupplierId,
            s.Name,
            s.Email,
            s.Phone,
            s.Address,
            s.SupplierIngredients.Count == 0))
            .ToList();

        return new SuppliersListResponse(list);
    }
}