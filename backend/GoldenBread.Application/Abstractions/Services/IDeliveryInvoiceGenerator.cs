using GoldenBread.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Abstractions.Services;

public interface IDeliveryInvoiceGenerator
{
    byte[] Generate(Order order, Company company);

}
