using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Common.Abstractions.Services;

public interface IPasswordHasher
{
    string GeneratePassword(string password);
    bool VerifyPassword(string providedPassword, string hashedPassword);
}
