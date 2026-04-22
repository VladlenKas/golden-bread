using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Features.Document.Dtos;

public class GenerateDeliveryInvoiceResponse
{
    public byte[] FileBytes { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
