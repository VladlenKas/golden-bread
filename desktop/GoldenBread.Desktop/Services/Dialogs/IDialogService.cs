using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services.Dialogs;

public interface IDialogService
{
    void ShowSuccess(ISukiDialogManager manager, string message);
    void ShowError(ISukiDialogManager manager, string message);
    void ShowInfo(ISukiDialogManager manager, string message);
    void ShowWarning(ISukiDialogManager manager, string message);
}
