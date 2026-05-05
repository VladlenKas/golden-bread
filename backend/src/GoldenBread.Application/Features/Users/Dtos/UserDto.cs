using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Users.Dtos;

public record class UserDto(
    int UserId,
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateOnly Birthday,
    UserRole Role,
    AccountType AccountType,
    VerificationStatus VerificationStatus);
