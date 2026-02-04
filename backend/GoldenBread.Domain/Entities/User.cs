using GoldenBread.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoldenBread.Domain.Entities;

public partial class User
{
    public int UserId { get; private set; }
    public int AccountId { get; private set; }
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateOnly Birthday { get; set; }
    public UserRole Role { get; set; } 
    public Account Account { get; set; } = null!;

    private User() { }

    public static User Create(
        string lastname,
        string firstname,
        DateOnly birthay,
        UserRole role,
        Account account,
        string? patronymic = null)
    {
        return new User
        {
            Lastname = lastname,
            Firstname = firstname,
            Patronymic = patronymic,
            Birthday = birthay,
            Role = role,
            Account = account
        };
    }

    public string GetFullName()
        => $"{Lastname} {Firstname} {Patronymic}".Trim();
}
