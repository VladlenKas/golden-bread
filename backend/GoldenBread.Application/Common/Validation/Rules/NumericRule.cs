using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Common.Validation.Rules;

public class NumericRule
{
    public static async Task<bool> IsNumeric(string value, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(value) &&
           value.All(char.IsDigit);
    }
}
