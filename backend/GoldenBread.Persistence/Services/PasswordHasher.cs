using GoldenBread.Application.Services;

namespace GoldenBread.Infrastructure.Services;

internal class PasswordHasher : IPasswordHasher
{
    public string Create(string password) =>
        BCryptNet.HashPassword(password);

    public bool Verify(string providedPassword, string hashedPassword) =>
        BCryptNet.Verify(providedPassword, hashedPassword);
}
