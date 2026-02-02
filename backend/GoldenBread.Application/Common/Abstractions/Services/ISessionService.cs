using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Common.Abstractions.Services;

public interface ISessionService
{
    string GenerateSessionId();
    DateTime GenerateSessionExpiry();
    bool IsSessionValid(string sessionId, DateTime expiresAt);
}
