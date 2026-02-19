using GoldenBread.Application.Features.CompanyAccount.Dtos;

namespace GoldenBread.Application.Features.Company.Queries.GetById;

public sealed record class GetByIdQuery(int Id) : 
    IRequest<CompanyResponse>;

