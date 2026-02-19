using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public sealed class Account
{
    public int AccountId { get; private set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Session { get; set; }
    public DateTime? SessionExpiresAt { get; set; }
    public short IsActive { get; set; }

    public AccountType AccountType { get; set; }
    public VerificationStatus VerificationStatus { get; set; }

    public User User { get; set; } = null!;
    public Company Company { get; set; } = null!;

    private Account() { }

    public static Account Create(
        string email,
        string passwordHash,
        AccountType accountType,
        string? session = null,
        DateTime? sessionExpiresAt = null)
    {
        return new Account
        {
            Email = email,
            PasswordHash = passwordHash,
            AccountType = accountType,
            VerificationStatus = VerificationStatus.Pending,
            Session = session,
            SessionExpiresAt = sessionExpiresAt,
            IsActive = 1
        };
    }

    public void UpdateEmail(string email)
    {
        Email = email;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void SetPendingVerification()
    {
        VerificationStatus = VerificationStatus.Pending;
    }

    public void SetSession()
    {
        Session = Guid.NewGuid().ToString("N");
        SessionExpiresAt = DateTime.UtcNow.AddDays(1);
    }

    public void ClearSession() 
    { 
        Session = null;
        SessionExpiresAt = null;
    }
}

