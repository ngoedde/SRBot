using System.Security.Cryptography;
using ReactiveUI;
using SRCore.Config;
using SRCore.Config.Model;

namespace SRCore.Components;

public class AccountService(ConfigService configService) : ReactiveObject
{
    public AutoLoginConfig Config => configService.GetConfig<AutoLoginConfig>() ?? new AutoLoginConfig();

    public async Task Initialize(string vaultPassword)
    {
        _= await configService.LoadConfigurationAsync(AutoLoginConfig.FileName, new AutoLoginConfig());
        
        Config.RaisePropertyChanged(nameof(Config.Accounts));
    }
    
    public int AddAccount(string username, string password, string secondaryPassword)
    {
        var accountId = RandomNumberGenerator.GetInt32(1000, 2000);

        Config.AddAccount(accountId, username, password, secondaryPassword);
        
        return accountId;
    }

    public async Task SaveAsync()
    {
        await configService.SaveAsync(AutoLoginConfig.FileName);
    }
    
    public AccountInfo? GetAccount(int id)
    {
        return Config?.Accounts.FirstOrDefault(a => a.Id == id);
    }
    
    public void RemoveAccount(AccountInfo account)
    {
        Config.Accounts.Remove(account);
        
        this.RaisePropertyChanged(nameof(Config));
    }
    
    public void ClearAccounts()
    {
        Config.Accounts.Clear();
        
        this.RaisePropertyChanged(nameof(Config));
    }
}