using GoldenBread.Desktop.UI.Common;

namespace GoldenBread.Desktop.Features.References.Employees;

public class EmployeesPageViewModel(
    string displayName, 
    string pageKey, 
    int order = 0) : PageViewModelBase(displayName, pageKey, order)
{

}
