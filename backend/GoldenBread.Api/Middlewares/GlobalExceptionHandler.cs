using Microsoft.AspNetCore.Diagnostics;

namespace GoldenBread.Api.Middlewares;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = exception.GetType().Name,
            Instance = httpContext.Request.Path,
            Extensions = GetExtension(exception)
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        ValidationException => (StatusCodes.Status400BadRequest, "One or more validation errors has occurred"),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
    };

    private static Dictionary<string, object?> GetExtension(Exception exception) => exception switch
    {
        ValidationException vEx => new Dictionary<string, object?> { ["errors"] = vEx.Errors },
        _ => new Dictionary<string, object?> { ["message"] = exception.Message }
    };
}
