using GoldenBread.Application.Repositories;
using GoldenBread.Domain.Entities;
using GoldenBread.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Repositories;

public class ProductRepository(GoldenBreadContext context) : IProductRepository
{
    public async Task AddAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<User?>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task SoftDeleteAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Product product)
    {
        throw new NotImplementedException();
    }
}
