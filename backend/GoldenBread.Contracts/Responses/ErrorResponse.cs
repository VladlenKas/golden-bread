using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Contracts.Responses;

public class ErrorResponse
{
    public required string Message { get; init; }
    public required DateTime Timestamp { get; set; }
}
