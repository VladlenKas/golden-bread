using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Common.Abstractions.Services;

public interface ICurrentUserService
{
    Task<Account?> Account(CancellationToken cancellationToken);
}
