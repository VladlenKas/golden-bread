namespace GoldenBread.Application.Features.CompanyProfile.Commands.UpdateContacts;

public sealed record class UpdateContactsCommand(
    string? Phone,
    string? Address) : IRequest<Unit>;
