using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Common.Abstractions.Services;

public interface IPasswordHasher
{
    string Create(string password);
    bool Verify(string providedPassword, string hashedPassword);
}
