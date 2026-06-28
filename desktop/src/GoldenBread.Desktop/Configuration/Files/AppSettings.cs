namespace GoldenBread.Desktop.Configuration.Files;

public static class AppSettings
{
    // Для разработки - сменить флаг на true
    public static bool IsDevelopment { get; set; } = false;

    public static string ApiUrl => IsDevelopment 
        ? "https://localhost:7107" 
        : "http://localhost:5000";
    
    public static string ApiDbUploadUrl => IsDevelopment 
        ? "https://localhost:7107/uploads" 
        : "http://localhost:5000/uploads";

    public const string PagesJson = "avares://GoldenBread.Desktop/Configuration/Files/pages.json";
    public const string RolesJson = "avares://GoldenBread.Desktop/Configuration/Files/roles.json";
}
