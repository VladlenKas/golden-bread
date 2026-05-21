using GoldenBread.Desktop.UI.Helpers;
using Refit;
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

            // 1. Ищем наши кастомные поля (приоритет: message > error > detail)
            var customText = GetString(root, "message")
                ?? GetString(root, "error")
                ?? GetString(root, "detail");

            // 2. Берем title/type, но отсекаем generic ASP.NET
            var title = GetString(root, "title");
            var type = GetString(root, "type");

            var isGenericTitle = title is not null && (
                title.Equals("Conflict", StringComparison.OrdinalIgnoreCase) ||
                title.Equals("Bad Request", StringComparison.OrdinalIgnoreCase) ||
                title.Equals("Not Found", StringComparison.OrdinalIgnoreCase) ||
                title.Equals("Unauthorized", StringComparison.OrdinalIgnoreCase) ||
                title.Contains("validation", StringComparison.OrdinalIgnoreCase) ||
                title.Contains("unexpected", StringComparison.OrdinalIgnoreCase));

            // 3. Формируем базовый текст
            var mainText = customText;
            if (string.IsNullOrWhiteSpace(mainText) && !isGenericTitle)
                mainText = title;
            if (string.IsNullOrWhiteSpace(mainText) && type is not null)
                mainText = GetMessageByExceptionType(type);

            // 4. Если есть shortages — значит это 409, добавляем список ингредиентов
            if (root.TryGetProperty("shortages", out var shortages) && shortages.ValueKind == JsonValueKind.Array)
            {
                var lines = new List<string>();

                // Если API не прислал message/error — берем дефолт по типу
                if (string.IsNullOrWhiteSpace(mainText))
                    mainText = "Недостаточно ингредиентов для запуска заказа в производство\n";

                lines.Add(mainText);

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

            return mainText ?? apiEx.Message;
        }
        catch
        {
            return apiEx.Message;
        }

        static string? GetString(JsonElement el, string prop) =>
            el.TryGetProperty(prop, out var p) && p.ValueKind == JsonValueKind.String
                ? p.GetString()
                : null;
    }

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
}
