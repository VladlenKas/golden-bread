namespace GoldenBread.Application.Features.CompanyProfile.Commands.UpdateRequisites;

public sealed record class UpdateRequisitesCommand(
    string Name,
    string Inn,
    string Ogrn) : IRequest<Unit>;