using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Domain.Entities;
using GoldenBread.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Repositories;

public class UserRepository(GoldenBreadContext context) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<User?>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }
}
