using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Contracts.Requests;

public class RegisterCompanyRequest
{
    public required string Name { get; set; } 
    public required string Inn { get; set; }
    public required string Ogrn { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
