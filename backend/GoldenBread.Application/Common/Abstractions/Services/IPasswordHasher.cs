namespace GoldenBread.Application.Common.Abstractions.Services;

public interface IPasswordHasher
{
    string Create(string password);
    bool Verify(string providedPassword, string hashedPassword);
}
