using GoldenBread.Application.Services;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GoldenBread.Infrastructure.Services;

internal class PasswordHasher : IPasswordHasher
{
    public string Create(string password) =>
        BCryptNet.HashPassword(password);

    public bool Verify(string providedPassword, string hashedPassword) =>
        BCryptNet.Verify(providedPassword, hashedPassword);
}
