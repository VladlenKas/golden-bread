using GoldenBread.Contracts.Responses;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public sealed record RegisterCompanyCommand(
    string Email,
    string Password,
    string Name,
    string Inn,
    string Ogrn) : IRequest<RegisterCompanyResponse>;
