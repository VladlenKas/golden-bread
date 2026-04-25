using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.UI.Common;

public partial class PageViewModelBase : ReactiveObject, INotifyDataErrorInfo
{
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    [Reactive] private string _displayName = string.Empty;
    [Reactive] private string _pageKey = string.Empty;
    [Reactive] private int _order;

    public bool HasErrors
    {
        get
        {
            var context = new ValidationContext(this);
            var result = new List<ValidationResult>();
            return !Validator.TryValidateObject(this, context, result, true);
        }
    }

    public IEnumerable GetErrors(string? propertyName) => Enumerable.Empty<string>();
}
