using GoldenBread.Domain.Constants;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Exceptions;

namespace GoldenBread.Domain.Entities;

public sealed class Account
{
    public int AccountId { get; private set; }

    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string? Session { get; private set; }
    public DateTime? SessionExpiresAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AccountType AccountType { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; }

    public User? User { get; private set; }
    public Company? Company { get; private set; }

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
            DeletedAt = null
        };
    }

    public Company GetCompany()
    {
        return Company ?? 
            throw new DomainExceptions(nameof(Company), ValidationErrorConstants.AccountHasNoCompany);
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

