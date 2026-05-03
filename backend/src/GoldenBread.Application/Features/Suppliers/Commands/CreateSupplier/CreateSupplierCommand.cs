using GoldenBread.Application.Features.Suppliers.Dtos;

namespace GoldenBread.Application.Features.Suppliers.Commands.CreateSupplier;

public sealed record CreateSupplierCommand(SupplierDto SupplierDto) : IRequest<int>;