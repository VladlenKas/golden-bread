using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.Users.Dtos;

namespace GoldenBread.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(query.Id, ct);
        if (user is null || user.Account is null)
            return null;

        return new UserDto(
            user.UserId,
            user.Firstname,
            user.Lastname,
            user.Patronymic,
            user.Birthday,
            user.Role,
            user.Account.AccountType,
            user.Account.VerificationStatus);
    }
}
