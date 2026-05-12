using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Users.Dtos;

public record UsersListResponse(List<UserListItem> UsersList);

public record UserListItem(
    int AccountId,
    int UserId,
    string Fullname,
    DateOnly Birthday,
    UserRole Role,
    string Email,
    VerificationStatus VerificationStatus,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SessionExpiresAt);
