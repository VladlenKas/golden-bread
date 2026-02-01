using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Api.Extensions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode = StatusCodes.Status500InternalServerError; 
        string message = "Внутренняя ошибка сервера";

        // 409 errors
        if (exception is EmailAlreadyExistsException ex)
        {
            statusCode = StatusCodes.Status409Conflict;
            message = ex.Message;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Message = message,
            Timestamp = DateTime.Now
        }, cancellationToken: cancellationToken);

        return true;
    }
}
