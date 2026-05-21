using GoldenBread.Application.Common.Exceptions;
using GoldenBread.Application.Features.Orders.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace GoldenBread.Api.Handlers;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct)
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
        AuthException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
        ValidationException => (StatusCodes.Status422UnprocessableEntity, "One or more validation errors has occurred"),
        DuplicateEntityException ex => (StatusCodes.Status409Conflict, $"Error duplicating the value for the \"{ex.PropertyName}\" parameter"),
        BusinessValidationException => (StatusCodes.Status422UnprocessableEntity, "One validation error has occurred"),
        NotFoundException => (StatusCodes.Status404NotFound, "Not found resourse"),
        InsufficientIngredientsException ex => (StatusCodes.Status409Conflict, ex.Message),
        InvalidOperationException => (StatusCodes.Status400BadRequest, "Bad Request"),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
    };

    private static Dictionary<string, object?> GetExtension(Exception exception) => exception switch
    {
        ValidationException ex => new Dictionary<string, object?> { ["message"] = exception.Message, ["errors"] = ex.Errors },
        DuplicateEntityException ex => new Dictionary<string, object?> { ["message"] = exception.Message, ["errors"] = ex.Error },
        BusinessValidationException ex => new Dictionary<string, object?> { ["message"] = exception.Message, ["errors"] = ex.Error },
        InsufficientIngredientsException ex => new Dictionary<string, object?> { ["message"] = exception.Message, ["shortages"] = ex.Shortages },
        _ => new Dictionary<string, object?> { ["message"] = exception.Message }
    };
}
