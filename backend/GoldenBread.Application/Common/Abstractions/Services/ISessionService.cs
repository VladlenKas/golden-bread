using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Common.Abstractions.Services;

public interface ISessionService
{
    (string session, DateTime sessionExpiresAt) Create();
    bool IsSessionValid(string sessionId, DateTime expiresAt);
}
