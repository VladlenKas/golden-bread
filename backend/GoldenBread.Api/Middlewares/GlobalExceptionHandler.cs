using GoldenBread.Application.Exceptions;
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
        ValidationException => (StatusCodes.Status422UnprocessableEntity, "One or more validation errors has occurred"),
        AccountNotFoundException => (StatusCodes.Status401Unauthorized, "Account not found"),
        SessionExpiredException => (StatusCodes.Status401Unauthorized, "Session not found or expired"),
        DuplicateValueException ex => (StatusCodes.Status409Conflict, $"Error duplicating the value for the \"{ex.PropertyName}\" parameter"),
        BusinessValidationException => (StatusCodes.Status422UnprocessableEntity, "One validation error has occurred"),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
    };

    private static Dictionary<string, object?> GetExtension(Exception exception) => exception switch
    {
        ValidationException ex => new Dictionary<string, object?> { ["message"] = exception.Message, ["errors"] = ex.Errors },
        DuplicateValueException ex => new Dictionary<string, object?> { ["message"] = exception.Message, ["errors"] = ex.Error },
        BusinessValidationException ex => new Dictionary<string, object?> { ["message"] = exception.Message, ["errors"] = ex.Error },
        _ => new Dictionary<string, object?> { ["message"] = exception.Message }
    };
}
