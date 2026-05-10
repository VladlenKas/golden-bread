using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Exceptions;
using GoldenBread.Domain.Entities;
using System.Security.Principal;

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

        if (await supplierRepository.ExistsByNameAsync(dto.Name, null, ct))
            throw new DuplicateEntityException(nameof(dto.Name));
        else if (await supplierRepository.ExistsByEmailAsync(dto.Email, null, ct))
            throw new DuplicateEntityException(nameof(dto.Email));
        else if (await supplierRepository.ExistsByPhoneAsync(dto.Phone, null, ct))
            throw new DuplicateEntityException(nameof(dto.Phone));
        else if (await supplierRepository.ExistsByAddressAsync(dto.Address, null, ct))
            throw new DuplicateEntityException(nameof(dto.Address));

        await supplierRepository.AddAsync(supplier, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return supplier.SupplierId;
    }
}