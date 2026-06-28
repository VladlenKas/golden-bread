using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Services;

public interface IDeliveryInvoiceGenerator
{
    byte[] Generate(Order order, Company company, User? issuedBy);
}
