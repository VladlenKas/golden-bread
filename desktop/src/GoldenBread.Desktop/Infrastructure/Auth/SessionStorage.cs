using Microsoft.AspNetCore.DataProtection;

namespace GoldenBread.Desktop.Infrastructure.Auth;

public sealed class SessionStorage
{
    private readonly IDataProtector _protector;
    private readonly string _filePath;

    public SessionStorage()
    {
        var provider = DataProtectionProvider.Create("GoldenBread");
        _protector = provider.CreateProtector("Session");

        _filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GoldenBread", "session.dat");
    }

    public void SaveSession(string session)
    {
        var encrypted = _protector.Protect(session);
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        File.WriteAllText(_filePath, encrypted);
    }

    public string? LoadSession()
    {
        if (!File.Exists(_filePath)) return null;
        var encrypted = File.ReadAllText(_filePath);
        return _protector.Unprotect(encrypted);
    }

    public void Clear() => File.Delete(_filePath);
}
