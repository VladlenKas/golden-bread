using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.Features.Common.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Features.Administration.Users.ViewModels;

public partial class ChangePasswordPageViewModel(
    IAccountApi api,
    ToastService toastService,
    DialogService dialogService) : PageViewModel, ISukiStackPageTitleProvider
{
    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = ConstantMessages.PasswordLengthValidation)]
    string? _password;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    string? _confirmPassword;

    [Reactive] private bool _isBusy;

    public int? AccountId { get; set; }
    public string Title { get; set; } = "Смена пароля";

    [ReactiveCommand]
    public async Task<bool> SaveAsync()
    {
        if (HasErrors)
        {
            toastService.ShowError(GetFirstError());
            return false;
        }
        else if (Password != ConfirmPassword)
        {
            toastService.ShowError(ConstantMessages.PasswordsMismatchValidation);
            return false;
        }

        var tcs = dialogService.ShowWarningQustion(ConstantMessages.UpdatePasswordConfirmDialog);

        bool confirmed = await tcs.Task;

        if (!confirmed)
            return false;

        IsBusy = true;
        try
        {
            if (AccountId == null)
            {
                toastService.ShowError(ConstantMessages.EmptySelectedItem);
                return false;
            }

            var response = await api.UpdatePassword(new UpdateAccountPasswordRequest(AccountId.Value, Password!));

            if (response.IsSuccessStatusCode)
            {
                toastService.ShowSuccess(ConstantMessages.SavedToast);
                return true;
            }

            toastService.ShowError();
            return false;
        }
        catch
        {
            dialogService.ShowError();
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    public async Task GoBackAsync() { }
}