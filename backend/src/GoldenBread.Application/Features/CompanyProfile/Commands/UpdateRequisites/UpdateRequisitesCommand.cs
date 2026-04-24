namespace GoldenBread.Application.Features.CompanyProfile.Commands.UpdateRequisites;

public sealed record UpdateRequisitesCommand(
    string Name,
    string Inn,
    string Ogrn) : IRequest<Unit>;