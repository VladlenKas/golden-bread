using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.Auth.Dtos;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Queries.GetAccountBySession;

public sealed class GetAccountBySessionQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentAccountContext accountContext) : 
    IRequestHandler<GetAccountBySessionQuery, AuthResponse?>
{
    public async Task<AuthResponse?> Handle(
        GetAccountBySessionQuery query, 
        CancellationToken ct)
    {
        // Если первый вход, возвращаем код 204
        if (!accountContext.HasSession)
            return null;

        // Если нет или сессия истекла, выбрасываем исключение с кодом 401
        var account = await accountContext.GetAccountAsync(ct);

        // Обновляем сессию
        account.SetSession();
        await unitOfWork.SaveChangesAsync(ct);

        // Всё ок? Возвращем код 200
        return AuthResponse.Response(account);
    }
}
