using GoldenBread.Application.Common.Exceptions.Validation;

namespace GoldenBread.Application.Common.Exceptions.Domain;

public abstract class BusinessValidationException : Exception
{
    public string PropertyName { get; }
    public List<ValidationError> Error { get; } = [];

    protected BusinessValidationException(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
        Error.Add(new ValidationError(propertyName, message));
    }
}

public class AccountHasNoCompanyException : BusinessValidationException
{
    public int AccountId { get; }

    public AccountHasNoCompanyException(int accountId)
        : base("Company", $"Account {accountId} has no associated company")
    {
        AccountId = accountId;
    }
}

public class InsufficientIngredientsException : BusinessValidationException
{
    public int IngredientId { get; }

    public InsufficientIngredientsException(int ingredientId)
        : base(nameof(IngredientId), $"Недостаточно ингредиентов (ID: {ingredientId})")
    {
        IngredientId = ingredientId;
    }
}