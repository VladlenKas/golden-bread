using GoldenBread.Application.Features.Auth.Dtos;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public sealed record RegisterCompanyCommand(
    string Email,
    string Password,
    string Name,
    string Inn,
    string Ogrn) : IRequest<AuthResponse>;
