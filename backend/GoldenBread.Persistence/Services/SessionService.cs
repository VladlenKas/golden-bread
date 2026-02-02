using GoldenBread.Application.Common.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Services;

internal class SessionService : ISessionService
{
    public string GenerateSessionId()
    {
        var animals = new[] { "cat", "dog", "fox", "bunny", "bear", "panda", "owl" };
        var colors = new[] { "tiny", "soft", "fluffy", "happy", "sweet", "cozy", "cute" };

        var animal = animals[Random.Shared.Next(animals.Length)];
        var color = colors[Random.Shared.Next(colors.Length)];
        var num = Random.Shared.Next(100, 999);

        return $"{color}-{animal}-{num}";
    }

    public DateTime GenerateSessionExpiry()
    {
        return DateTime.UtcNow.AddDays(7);
    }

    public bool IsSessionValid(string sessionId, DateTime expiresAt)
    {
        return !string.IsNullOrEmpty(sessionId) && expiresAt > DateTime.UtcNow;
    }
}
