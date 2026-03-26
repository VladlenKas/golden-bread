namespace GoldenBread.Application.Features.CompanyProfile.Commands.UpdateContacts;

public sealed record UpdateContactsCommand(
    string? Phone,
    string? Address) : IRequest<Unit>;
