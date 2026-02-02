using GoldenBread.Contracts.Responses;
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Api.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode = httpContext.Response.StatusCode; 
        string message = "Внутренняя ошибка сервера";

        await httpContext.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Message = message,
            Status = statusCode,
            Timestamp = DateTime.Now
        }, cancellationToken);

        return true;
    }
}
