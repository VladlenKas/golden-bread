using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public sealed class User
{
    public int UserId { get; private set; }
    public int? AccountId { get; private set; }

    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateOnly Birthday { get; set; }

    public UserRole Role { get; set; } 

    public Account? Account { get; set; } 

    private User() { }

    public static User Create(
        int userId,
        int accountId,
        string lastname,
        string firstname,
        string? patronymic,
        DateOnly birthay,
        UserRole role)
    {
        return new User
        {
            UserId = userId,
            AccountId = accountId,
            Lastname = lastname,
            Firstname = firstname,
            Patronymic = patronymic,
            Birthday = birthay,
            Role = role,
        };
    }

    public void Update(
        string firstname,
        string lastname,
        string? patronymic,
        DateOnly birthday,
        UserRole role)
    {
        Firstname = firstname;
        Lastname = lastname;
        Patronymic = patronymic;
        Birthday = birthday;
        Role = role;
    }

    public string Fullname => $"{Lastname} {Firstname} {Patronymic}".Trim();
}
