using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Users.Queries.GetUsersList;

public sealed class GetUsersListQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersListQuery, UsersListResponse>
{
    public async Task<UsersListResponse> Handle(GetUsersListQuery query, CancellationToken ct)
    {
        var usersAccounts = await userRepository.GetAllAsync(ct);

        var list = usersAccounts
            .Select(u => new UserListItem(
                u.AccountId,
                u.User!.UserId,
                u.User.Fullname,
                u.User.Birthday,
                u.User.Role,
                u.User.Account!.Email,
                u.VerificationStatus))
            .ToList();

        return new UsersListResponse(list);
    }
}
