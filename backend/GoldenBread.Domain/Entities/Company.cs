using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Domain.Entities;

public class Company
{
    public int CompanyId { get; set; }
    public int AccountId { get; set; }
    public string Name { get; set; } = null!;
    public string Inn { get; set; } = null!;
    public string Ogrn { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }

    // Navigation property
    public Account Account { get; set; } = null!;
}
