using ReactiveUI;
using SukiUI.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GoldenBread.Desktop.ViewModels;

public class ViewModelBase : ReactiveObject, INotifyDataErrorInfo
{
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public ISukiDialogManager DialogManager { get; } = new SukiDialogManager();

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
