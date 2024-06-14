using System.Linq;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using SRCore.Components;
using SRCore.Config.Model;
using SukiUI.Controls;
using SukiUI.Enums;

namespace SRBot.Dialog.GamePage;

public class AccountManagerDialogModel(AccountService accountService) : ViewModel
{
    public AccountService AccountService { get; } = accountService;

    [Reactive] public AccountInfo? SelectedAccount { get; set; } = accountService.Config.Accounts.FirstOrDefault();

    public void AddAccount()
    {
        var accountId = AccountService.AddAccount("MyUsername", "", "");

        SelectedAccount = AccountService.GetAccount(accountId);
    }

    public void DeleteAccount(AccountInfo? accountInfo)
    {
        if (accountInfo == null)
            return;

        accountService.RemoveAccount(accountInfo);
        if (SelectedAccount == accountInfo)
            SelectedAccount = AccountService.Config.Accounts.FirstOrDefault();
    }

    public async Task SaveAccounts()
    {
        await accountService.SaveAsync();
        SukiHost.ShowToast("Account Manager", "Accounts have been saved!", SukiToastType.Success);

        SukiHost.CloseDialog();
    }
}