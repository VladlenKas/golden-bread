using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Contracts.Requests;

public sealed record class RegisterCompanyRequest(
    string Name,
    string Inn,
    string Ogrn,
    string Email,
    string Password);
