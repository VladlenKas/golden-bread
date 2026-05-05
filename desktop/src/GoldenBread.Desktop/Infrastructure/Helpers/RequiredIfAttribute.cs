using GoldenBread.Desktop.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Infrastructure.Helpers;

public class RequiredIfAttribute(string propertyName, object expectedValue) : ValidationAttribute
{
    public string PropertyName { get; } = propertyName;
    public object ExpectedValue { get; } = expectedValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var instance = context.ObjectInstance;
        var property = instance.GetType().GetProperty(PropertyName);
        var actualValue = property?.GetValue(instance);

        // Если условие совпадает (UserId == 0) и значение пустое - ошибка
        if (actualValue?.Equals(ExpectedValue) == true && string.IsNullOrWhiteSpace(value as string))
        {
            return new ValidationResult(ErrorMessage ?? ConstantMessages.RequiredValidation);
        }

        return ValidationResult.Success;
    }
}
