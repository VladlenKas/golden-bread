using GoldenBread.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<User?>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task SoftDeleteAsync(Product product);
}
