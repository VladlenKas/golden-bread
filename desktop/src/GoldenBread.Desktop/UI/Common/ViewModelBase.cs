using ReactiveUI;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.UI.Common;

public class ViewModelBase : ReactiveObject, INotifyDataErrorInfo
{
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => GetAllErrors().Any();

    public IEnumerable GetErrors(string? propertyName) => Enumerable.Empty<string>();

    // Получить все ошибки через DataAnnotations
    public List<string> GetAllErrors()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, results, true);
        return results.Select(r => r.ErrorMessage!).ToList();
    }

    public string? GetFirstError() => GetAllErrors().FirstOrDefault();
}