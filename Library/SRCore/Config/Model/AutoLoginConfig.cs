using System.Collections.ObjectModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Config.Model;

public class AutoLoginConfig : ConfigElement
{
    public static string FileName = Path.Combine(Kernel.ConfigDirectory, "autologin.json");

    [Reactive] public ObservableCollection<AccountInfo> Accounts { get; set; } = new();

    public void AddAccount(int id, string username, string password, string secondaryPassword)
    {
        Accounts.Add(new AccountInfo(id)
        {
            Username = username,
            Password = password,
            SecondaryPassword = secondaryPassword
        });

        this.RaisePropertyChanged(nameof(Accounts));
    }
}