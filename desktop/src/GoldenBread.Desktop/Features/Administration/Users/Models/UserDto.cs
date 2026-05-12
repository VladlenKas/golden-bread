using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.Common.Account;

namespace GoldenBread.Desktop.Features.Administration.Users.Models;

public record class UserDto(
    int UserId,
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateOnly Birthday,
    UserRole Role,
    VerificationStatus VerificationStatus);