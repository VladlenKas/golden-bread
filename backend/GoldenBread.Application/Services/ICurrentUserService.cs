using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Services;

public interface ICurrentUserService
{
    Task<Account?> Account(CancellationToken cancellationToken);
}
