using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Users.Queries.GetUsersList;

public sealed class GetUsersListQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersListQuery, UsersListResponse>
{
    public async Task<UsersListResponse> Handle(GetUsersListQuery query, CancellationToken ct)
    {
        var usersAccounts = await userRepository.GetAllAsync(ct);

        var list = usersAccounts
            .Select(a => new UserListItem(
                a.AccountId,
                a.User!.UserId,
                a.User.Fullname,
                a.User.Birthday,
                a.User.Role,
                a.User.Account!.Email,
                a.VerificationStatus,
                a.CreatedAt,
                a.SessionExpiresAt))
            .ToList();

        return new UsersListResponse(list);
    }
}
