using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.Features.Common.Account;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Features.Common.ViewModels;

public partial class ChangeEmailPageViewModel(
    IAccountApi api,
    ToastService toastService,
    DialogService dialogService) : PageViewModel, ISukiStackPageTitleProvider
{
    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [RegularExpression(ConstantRegularExpressions.Email, ErrorMessage = ConstantMessages.EmailFormatValidation)]
    string? _email;

    [Reactive] private bool _isBusy;

    public int? AccountId { get; set; }
    public string Title { get; set; } = "Смена эл. почты";

    [ReactiveCommand]
    public async Task<bool> SaveAsync()
    {
        if (HasErrors)
        {
            toastService.ShowError(GetFirstError());
            return false;
        }

        var tcs = dialogService.ShowWarningQuestion(ConstantMessages.UpdateEmailConfirmDialog);

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

            var response = await api.UpdateEmail(new UpdateAccountEmailRequest(AccountId.Value, Email!));

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