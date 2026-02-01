using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Contracts.Responses;

public class LoginCompanyResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Session { get; set; }
    public required DateTime SessionExpiresAt { get; set; }
    public required string AccountStatus { get; set; }
}
