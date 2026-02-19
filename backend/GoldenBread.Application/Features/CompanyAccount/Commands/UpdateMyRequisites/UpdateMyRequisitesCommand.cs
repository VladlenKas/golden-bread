namespace GoldenBread.Application.Features.CompanyAccount.Commands.UpdateMyRequisites;

public sealed record class UpdateMyRequisitesCommand(
    string Name,
    string Inn,
    string Ogrn) : IRequest<Unit>;