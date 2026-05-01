using System.Diagnostics;

namespace GoldenBread.Desktop.Infrastructure.Auth;

public sealed class SessionHeaderHandler(SessionStorage storage) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken ct)
    {
        try
        {
            var session = storage.LoadSession();

            if (!string.IsNullOrEmpty(session))
                request.Headers.TryAddWithoutValidation("X-Desktop-Session", session);

            var response = await base.SendAsync(request, ct);
            return response;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[SessionHeaderHandler] Ошибка: {ex}");
            throw;
        }
    }
}