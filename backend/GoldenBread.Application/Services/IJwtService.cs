using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Services;

public interface IJwtService
{
    string GenerateToken(string accountId, string email, IEnumerable<Claim> claims);
    string? GenerateRefreshToken();
}
