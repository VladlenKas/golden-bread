namespace GoldenBread.Application.Features.CompanyAccount.Commands.UpdateMyContacts;

public sealed record class UpdateMyContactsCommand(
    string? Phone,
    string? Address) : IRequest<Unit>;
