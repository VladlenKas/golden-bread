using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Infrastructure.Helpers;

public class DateTimeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not DateTimeOffset dto)
            return new ValidationResult("Неверный тип даты");

        var maxDate = DateTimeOffset.Now;
        var minDate = maxDate.AddYears(-65);
        maxDate = maxDate.AddYears(-18);

        if (dto < minDate || dto > maxDate)
        {
            var message = ErrorMessage
                ?? $"Дата должна быть в промежутке от {minDate:dd.MM.yyyy} по {maxDate:dd.MM.yyyy}";
            return new ValidationResult(message);
        }

        return ValidationResult.Success;
    }
}