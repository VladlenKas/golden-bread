using GoldenBread.Application.Common.Abstractions.Services;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GoldenBread.Infrastructure.Services;

internal class PasswordHasher : IPasswordHasher
{
    public string GeneratePassword(string password) =>
        BCryptNet.HashPassword(password);

    public bool VerifyPassword(string providedPassword, string hashedPassword) =>
        BCryptNet.Verify(providedPassword, hashedPassword);
}
