using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Constants;
using GoldenBread.Application.Common.Exceptions;
using GoldenBread.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace GoldenBread.Infrastructure.Services;

internal class CurrentAccountContext(
    IHttpContextAccessor httpContextAccessor,
    IAccountRepository accountRepository) : 
    ICurrentAccountContext
{
    private Task<Account?>? _accountCache;

    private string? Session =>
        httpContextAccessor.HttpContext?.User.FindFirst("session")?.Value
        ?? httpContextAccessor.HttpContext?.Request.Headers["X-Desktop-Session"].FirstOrDefault();

    public bool HasSession =>
        httpContextAccessor.HttpContext?.Request.Cookies.ContainsKey("gb.session") == true ||
        !string.IsNullOrEmpty(httpContextAccessor.HttpContext?.Request.Headers["X-Desktop-Session"].FirstOrDefault());

    public async Task<Account> GetAccountAsync(CancellationToken ct)
    {
        _accountCache ??= accountRepository.GetBySessionAsync(Session, ct);
        return await _accountCache ?? throw new AuthException(
            ValidationErrorConstants.SessionExpired,
            AuthErrorType.ExpiredToken);
    }

    /// <summary>
    /// Только для защищённых маршрутов (с [Authorize]).
    /// </summary>
    public async Task<int> GetRequiredCompanyIdAsync(CancellationToken ct)
    {
        var account = await GetAccountAsync(ct);
        return account.GetCompany()!.CompanyId;
    }

    /// <summary>
    /// Для публичных маршрутов. Возвращает null, если пользователь не авторизован.
    /// </summary>
    public async Task<int?> GetCompanyIdAsync(CancellationToken ct)
    {
        if (Session == null)
            return null;

        var account = await GetAccountAsync(ct);
        return account.GetCompany()?.CompanyId;
    }
}
