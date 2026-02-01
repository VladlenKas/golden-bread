using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Common.Abstractions.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User?>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}
