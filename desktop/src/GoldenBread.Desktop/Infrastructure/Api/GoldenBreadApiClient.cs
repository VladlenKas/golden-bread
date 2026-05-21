using GoldenBread.Desktop.UI.Helpers;
using Refit;
using System.Net;
using System.Text.Json;

namespace GoldenBread.Desktop.Infrastructure.Api;

public class GoldenBreadApiClient(HttpClient httpClient)
{
    public HttpClient HttpClient => httpClient;

    public static string GetErrorMessage(ApiException apiEx)
    {
        if (string.IsNullOrWhiteSpace(apiEx.Content))
            return apiEx.Message;

        try
        {
            using var doc = JsonDocument.Parse(apiEx.Content);
            var root = doc.RootElement;

            var type = GetString(root, "type");
            var title = GetString(root, "title");

            // --- 1. ОШИБКИ ВАЛИДАЦИИ / ДУБЛИКАТЫ (массив errors) ---
            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array)
            {
                var lines = new List<string>();

                foreach (var item in errors.EnumerateArray())
                {
                    var prop = item.TryGetProperty("propertyName", out var p) ? p.GetString() : null;
                    var msg = item.TryGetProperty("errorMessage", out var m) ? m.GetString() : null;

                    if (!string.IsNullOrWhiteSpace(prop))
                    {
                        var ruProp = PropertyNamesRu.GetValueOrDefault(prop, prop);
                        lines.Add($"• {ruProp}: {msg ?? "уже используется"}");
                    }
                    else if (!string.IsNullOrWhiteSpace(msg))
                    {
                        lines.Add($"• {msg}");
                    }
                }

                if (lines.Count > 0)
                {
                    bool isDuplicate =
                        type?.Contains("Duplicate", StringComparison.OrdinalIgnoreCase) == true ||
                        title?.Contains("duplicating", StringComparison.OrdinalIgnoreCase) == true ||
                        apiEx.StatusCode == HttpStatusCode.Conflict;

                    if (isDuplicate)
                    {
                        return lines.Count == 1
                            ? "Дублирование: " + lines[0].TrimStart('•', ' ')
                            : "Обнаружено дублирование:\n" + string.Join("\n", lines);
                    }

                    // Обычная валидация (не дубликат)
                    var header = GetString(root, "message") ?? "Ошибки валидации:";
                    return header + "\n" + string.Join("\n", lines);
                }
            }

            // --- 2. SHORTAGES (как было раньше) ---
            if (root.TryGetProperty("shortages", out var shortages) && shortages.ValueKind == JsonValueKind.Array)
            {
                var lines = new List<string>();
                var header = GetString(root, "message")
                    ?? GetMessageByExceptionType(type)
                    ?? "Недостаточно ингредиентов";

                lines.Add(header);

                foreach (var item in shortages.EnumerateArray())
                {
                    var name = item.TryGetProperty("ingredientName", out var n) ? n.GetString() : "?";
                    var req = item.TryGetProperty("requiredQuantity", out var r) ? r.GetDecimal() : 0;
                    var avail = item.TryGetProperty("availableQuantity", out var a) ? a.GetDecimal() : 0;
                    var unit = item.TryGetProperty("unit", out var u) ? u.GetString() ?? "" : "";

                    lines.Add($"• {name}: нужно {req} {LocalizedIngredientUnits.UnitsDetail(unit)}, есть {avail} {LocalizedIngredientUnits.UnitsDetail(unit)}");
                }

                return string.Join("\n", lines);
            }

            // --- 3. БАЗОВЫЙ ТЕКСТ (message / error / detail / title / type) ---
            var mainText = GetString(root, "message")
                ?? GetString(root, "error")
                ?? GetString(root, "detail");

            if (string.IsNullOrWhiteSpace(mainText) && title is not null && !IsGenericTitle(title))
                mainText = title;

            if (string.IsNullOrWhiteSpace(mainText) && type is not null)
                mainText = GetMessageByExceptionType(type);

            return mainText ?? apiEx.Message;
        }
        catch
        {
            return apiEx.Message;
        }
    }

    private static bool IsGenericTitle(string title) =>
        title.Equals("Conflict", StringComparison.OrdinalIgnoreCase) ||
        title.Equals("Bad Request", StringComparison.OrdinalIgnoreCase) ||
        title.Equals("Not Found", StringComparison.OrdinalIgnoreCase) ||
        title.Equals("Unauthorized", StringComparison.OrdinalIgnoreCase) ||
        title.Contains("validation", StringComparison.OrdinalIgnoreCase) ||
        title.Contains("unexpected", StringComparison.OrdinalIgnoreCase);

    private static string? GetMessageByExceptionType(string type) => type switch
    {
        "InvalidOperationException" => "Невозможно распределить задачи для заказа",
        "InsufficientIngredientsException" => "Недостаточно ингредиентов для запуска заказа в производство",
        "NotFoundException" => "Ресурс не найден",
        "ValidationException" => "Ошибка валидации",
        "DuplicateEntityException" => "Дублирование сущности",
        "BusinessValidationException" => "Бизнес-ошибка",
        _ => null
    };


    /// <summary>
    /// Перевод имён свойств на русский для тостов
    /// Ключи должны совпадать с тем, что присылает API в propertyName
    /// </summary>
    private static readonly Dictionary<string, string> PropertyNamesRu = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Name"] = "Название",
        ["NewEmail"] = "Электронная почта",
        ["Role"] = "Роль",
        ["Phone"] = "Телефон",
        ["Address"] = "Адрес",
        ["UserName"] = "Логин",
        ["Password"] = "Пароль",
        ["SupplierId"] = "Поставщик",
        ["IngredientId"] = "Ингредиент",
        ["ProductId"] = "Продукт",
        ["OrderId"] = "Заказ",
        ["AccountId"] = "Аккаунт",
        ["Role"] = "Роль",
    };

    private static string? GetString(JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var p) && p.ValueKind == JsonValueKind.String
            ? p.GetString()
            : null;
}
