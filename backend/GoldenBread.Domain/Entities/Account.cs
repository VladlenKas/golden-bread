using GoldenBread.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Domain.Entities;

public partial class Account
{
    public int AccountId { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public AccountType AccountType { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public string? Session { get; set; }
    public DateTime? SessionExpiresAt { get; set; }
    public short IsActive { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Company Company { get; set; } = null!;
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

