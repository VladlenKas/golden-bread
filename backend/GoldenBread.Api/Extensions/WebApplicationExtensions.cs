using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;

namespace GoldenBread.Api.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseExceptionHandler();

        app.UseHttpsRedirection();

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
    