using GoldenBread.Api.Handlers;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;

namespace GoldenBread.Api.Configuration;

public static class WebApplicationExtensions
{
  public static WebApplication UseApi(this WebApplication app)
  {
    if (app.Environment.IsDevelopment())
    {
      app.MapOpenApi();
      app.MapScalarApiReference();
    }

    app.UseCors(builder =>
    {
        var origins = app.Configuration.GetSection("Cors:Origins").Get<string[]>()
            ?? new[] { "https://localhost:7107", "https://localhost:5173" };

        builder.WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });

    app.UseExceptionHandler();
    app.UseMiddleware<DesktopAuthMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    var uploadsPath = app.Configuration["Data:DbUploadsPath"]!;

    if (!Path.IsPathRooted(uploadsPath))
    {
        // ContentRootPath - корень проекта при dotnet run, /app при Docker
        uploadsPath = Path.GetFullPath(
            Path.Combine(app.Environment.ContentRootPath, uploadsPath));
    }

    // PhysicalFileProvider требует, чтобы папка существовала
    Directory.CreateDirectory(uploadsPath);

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uploadsPath),
        RequestPath = "/uploads"
    });

    return app;
  }
}
