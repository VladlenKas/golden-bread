using GoldenBread.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoldenBread.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }
    public int AccountId { get; set; }
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateOnly Birthday { get; set; }
    public UserRole Role { get; set; } 

    // Navigation property
    public Account Account { get; set; } = null!;
}
