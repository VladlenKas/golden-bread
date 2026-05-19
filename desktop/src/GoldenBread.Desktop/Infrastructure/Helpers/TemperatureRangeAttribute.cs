using GoldenBread.Desktop.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Infrastructure.Helpers;

public class TemperatureRangeAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public TemperatureRangeAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var currentValue = (decimal?)value;
        var property = context.ObjectType.GetProperty(_comparisonProperty);
        var comparisonValue = (decimal?)property?.GetValue(context.ObjectInstance);

        if (currentValue.HasValue && comparisonValue.HasValue && currentValue >= comparisonValue)
        {
            return new ValidationResult(ConstantMessages.TemperatureRangeValidation);
        }

        return ValidationResult.Success;
    }
}