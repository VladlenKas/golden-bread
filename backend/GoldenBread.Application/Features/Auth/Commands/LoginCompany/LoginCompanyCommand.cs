using GoldenBread.Contracts.Responses;

namespace GoldenBread.Application.Features.Auth.Commands.LoginCompany;

public sealed record class LoginCompanyCommand(
    string Email,
    string Password) : IRequest<LoginCompanyResponse?>;
