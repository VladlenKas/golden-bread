using GoldenBread.Application.Common.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Services;

internal class NumericValidator : INumericValidator
{
    public async Task<bool> IsNumeric(string input, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(input) &&
           input.All(char.IsDigit);
    }
}
