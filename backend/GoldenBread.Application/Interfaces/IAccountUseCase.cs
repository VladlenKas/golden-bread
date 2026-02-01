using GoldenBread.Contracts.Requests;
using GoldenBread.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Interfaces;

public interface IAccountUseCase
{
    Task<LoginUserResponse?> LoginUserAsync(LoginRequest request);
    Task<LoginCompanyResponse?> LoginCompanyAsync(LoginRequest request);
    Task<RegisterCompanyResponse> RegisterCompanyAsync(RegisterCompanyRequest request);
}
