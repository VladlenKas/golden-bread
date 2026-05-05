using GoldenBread.Desktop.Features.Common.Models;

namespace GoldenBread.Desktop.UI.Helpers;

public class LocalizedAccountType
{
    public Dictionary<AccountType, string> AccountTypeLocalizations { get; } = new()
    {
        [AccountType.User] = "Пользователь",
        [AccountType.Company] = "Компания"
    };
}
