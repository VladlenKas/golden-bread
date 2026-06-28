using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Services;

public interface ICooperationAgreementGenerator
{
    byte[] Generate(Company buyer);
}