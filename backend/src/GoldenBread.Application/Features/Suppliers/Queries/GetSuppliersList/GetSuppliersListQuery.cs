using GoldenBread.Application.Features.Suppliers.Dtos;

namespace GoldenBread.Application.Features.Suppliers.Queries.GetSuppliersList;

public sealed record GetSuppliersListQuery : IRequest<SuppliersListResponse>;