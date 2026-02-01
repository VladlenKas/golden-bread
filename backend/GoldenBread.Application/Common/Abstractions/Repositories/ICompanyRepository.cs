using GoldenBread.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Common.Abstractions.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Company?>> GetAllAsync();
    Task<Company?> GetByIdAsync(int id);
    Task<Company?> GetByNameAsync(string name);
    Task<Company?> GetByInnAsync(string inn);
    Task<Company?> GetByOgrnAsync(string ogrn);
    Task<Company?> GetByPhoneAsync(string phone);
    Task<Company?> GetByAddressAsync(string address);
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task DeleteAsync(Company company);
}
