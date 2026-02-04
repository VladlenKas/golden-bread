using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Common.Abstractions.Data;

public interface IGoldenBreadContext
{
    DbSet<Account> Accounts { get; set; }
    DbSet<Company> Companies { get; set; }
    DbSet<User> Users { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
