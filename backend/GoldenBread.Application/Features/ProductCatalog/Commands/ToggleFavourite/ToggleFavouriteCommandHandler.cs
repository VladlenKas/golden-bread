using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.ProductCatalog.Commands.ToggleFavourite;

public class ToggleFavouriteCommandHandle(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext) : 
    IRequestHandler<ToggleFavouriteCommand, Unit>
{
    public async Task<Unit> Handle(
        ToggleFavouriteCommand command, 
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);

        int companyId = (await context.Companies
            .FirstAsync(c => 
                c.AccountId == account.AccountId,
                cancellationToken))
            .CompanyId;

        var favourite = context.Favourites
            .FirstOrDefault(f =>
                f.ProductId == command.ProductId &&
                f.CompanyId == companyId);

        if (favourite == null)
        {
            favourite = Favourite.Create(
                companyId,
                command.ProductId);

            await context.Favourites.AddAsync(favourite, cancellationToken);
        }
        else
        {
            context.Favourites.Remove(favourite);
        }

        return Unit.Value;  
    }
}
