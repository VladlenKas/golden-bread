using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Common.Abstractions.Services;

public interface INumericValidator
{
    Task<bool> IsNumeric(string input, CancellationToken cancellationToken);
}
