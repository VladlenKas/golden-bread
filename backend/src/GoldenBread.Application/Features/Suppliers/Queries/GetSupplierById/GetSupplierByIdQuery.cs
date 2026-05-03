using GoldenBread.Application.Features.Suppliers.Dtos;

namespace GoldenBread.Application.Features.Suppliers.Queries.GetSupplierById;

public sealed record GetSupplierByIdQuery(int Id) : IRequest<SupplierDto?>;