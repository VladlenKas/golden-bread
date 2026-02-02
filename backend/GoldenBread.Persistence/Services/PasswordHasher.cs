using GoldenBread.Application.Common.Abstractions.Services;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GoldenBread.Infrastructure.Services;

internal class PasswordHasher : IPasswordHasher
{
    public string GeneratePassword(string password)
    {
        return BCryptNet.HashPassword(password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return BCryptNet.Verify(hashedPassword, providedPassword);
    }
}
