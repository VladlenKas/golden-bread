namespace GoldenBread.Desktop.Infrastructure.Api;

public class GoldenBreadApiClient(HttpClient httpClient)
{
    public HttpClient HttpClient => httpClient;
}
