using GoldenBread.Application.Features.Suppliers.Dtos;

namespace GoldenBread.Application.Features.Suppliers.Commands.UpdateSupplier;

public sealed record UpdateSupplierCommand(SupplierDto SupplierDto) : IRequest<bool>;