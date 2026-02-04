using GoldenBread.Application.Common.Abstractions.Services;

namespace GoldenBread.Infrastructure.Services;

internal class SessionService : ISessionService
{
    public (string session, DateTime sessionExpiresAt) GenerateSession()
    {
        var animals = new[] { "cat", "dog", "fox", "bunny", "bear", "panda", "owl" };
        var colors = new[] { "tiny", "soft", "fluffy", "happy", "sweet", "cozy", "cute" };

        var animal = animals[Random.Shared.Next(animals.Length)];
        var color = colors[Random.Shared.Next(colors.Length)];
        var num = Random.Shared.Next(100, 999);

        var session = $"{color}-{animal}-{num}";
        var sessionExpiresAt = DateTime.UtcNow.AddDays(7);

        return (session, sessionExpiresAt);
    }

    public bool IsSessionValid(string sessionId, DateTime expiresAt)
    {
        return !string.IsNullOrEmpty(sessionId) && expiresAt > DateTime.UtcNow;
    }
}
