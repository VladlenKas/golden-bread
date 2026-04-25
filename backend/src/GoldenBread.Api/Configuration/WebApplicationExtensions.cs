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
            builder.WithOrigins("https://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

        app.UseExceptionHandler();
        app.UseMiddleware<DesktopAuthMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(app.Configuration["Data:DbUploadsPath"]!),
            RequestPath = "/db_uploads"
        });

        return app;
    }
}
    