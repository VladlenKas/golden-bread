using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.UI.Services.Tosts;

public interface IToastService
{
    void ShowSuccess(ISukiToastManager manager, string message);
    void ShowError(ISukiToastManager manager, string message);
    void ShowInfo(ISukiToastManager manager, string message);
    void ShowWarning(ISukiToastManager manager, string message);
}
