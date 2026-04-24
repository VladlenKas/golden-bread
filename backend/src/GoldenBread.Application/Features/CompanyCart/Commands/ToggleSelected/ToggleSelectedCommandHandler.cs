using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;

namespace GoldenBread.Application.Features.CompanyCart.Commands.ToggleSelected;


public sealed class ToggleSelectedCommandHandler(
    IUnitOfWork unitOfWork,
    ICartRepository cartRepository,
    ICurrentAccountContext accountContext) :
    IRequestHandler<ToggleSelectedCommand, Unit>
{
    public async Task<Unit> Handle(
        ToggleSelectedCommand command,
        CancellationToken ct)
    {
        int companyId = await accountContext.GetRequiredCompanyIdAsync(ct);

        await cartRepository.ToggleSelectedAsync(command.ProductId, companyId, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

