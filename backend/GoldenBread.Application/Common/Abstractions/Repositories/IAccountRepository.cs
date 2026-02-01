using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Common.Abstractions.Repositories;

public interface IAccountRepository
{
    Task<IEnumerable<Account?>> GetAllAsync();
    Task<Account?> GetByEmailAsync(string email);
    Task<Account?> GetByIdAsync(int id);
    Task AddAsync(Account account);
    Task UpdateAsync(Account account);
    Task DeleteAsync(Account account);  
}
