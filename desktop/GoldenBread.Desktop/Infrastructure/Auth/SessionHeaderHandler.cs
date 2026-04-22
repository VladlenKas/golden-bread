using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Infrastructure.Auth;

public sealed class SessionHeaderHandler(
    ISessionStorage storage) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        var session = storage.LoadSession();

        if (!string.IsNullOrEmpty(session))
            request.Headers.TryAddWithoutValidation("X-Desktop-Session", session);

        return await base.SendAsync(request, ct);
    }
}