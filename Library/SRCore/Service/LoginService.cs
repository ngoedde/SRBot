using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using SRCore.Components;
using SRCore.Config;
using SRCore.Config.Model;
using SRCore.Models;
using SRCore.Models.ShardInfo;
using SRGame.Client;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Service;

public class LoginService(IServiceProvider serviceProvider)
{
    private Kernel Kernel => serviceProvider.GetRequiredService<Kernel>();
    private Proxy Proxy => serviceProvider.GetRequiredService<Proxy>();
    private ConfigService ConfigService => serviceProvider.GetRequiredService<ConfigService>();
    private ShardList ShardList => serviceProvider.GetRequiredService<ShardList>();
    private AccountService AccountService => serviceProvider.GetRequiredService<AccountService>();
    private ClientInfoManager ClientInfoManager => serviceProvider.GetRequiredService<ClientInfoManager>();
    private CharacterLobby CharacterLobby => serviceProvider.GetRequiredService<CharacterLobby>();
    private GameConfig GameConfig => ConfigService.GetConfig<GameConfig>() ?? new GameConfig();
    private AgentLogin AgentLogin => serviceProvider.GetRequiredService<AgentLogin>();

    public void Initialize()
    {
        ShardList.ShardListUpdated += ShardListOnShardListUpdated;
        CharacterLobby.CharacterListUpdated += CharacterLobbyOnCharacterListUpdated;
    }
    
    public void Login(string username, string password, Shard shard)
    {
        var msg = new Packet(GatewayMsgId.LoginReq, true);
        var contentId = ClientInfoManager.DivisionInfo.ContentId;
        
        msg.WriteByte(contentId);
        msg.WriteString(username);
        msg.WriteString(password);
        msg.WriteUShort(shard.Id);
        
        // Save login info for agent server
        AgentLogin.Username = username;
        AgentLogin.Password = password;
        AgentLogin.ShardId = shard.Id;
        AgentLogin.ContentId = contentId;
        
        Proxy.SendToServer(msg);
    }
    
    public void Logout()
    {
        
    }        


    public void SolveCaptcha(string captcha)
    {
        
    }
    
    /// <summary>
    /// Auto login: Join game after the character list has been received.
    /// </summary>
    /// <param name="characterLobby"></param>
    private void CharacterLobbyOnCharacterListUpdated(CharacterLobby characterLobby)
    {
        if (!GameConfig.EnableAutoLogin)
            return;
        
        var character = characterLobby.Characters.FirstOrDefault(c => string.Equals(c.Name, GameConfig.AutoLoginCharacter, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(GameConfig.AutoLoginCharacter));
        if (character == null)
        {
            _ = Kernel.TriggerInterrupt("Auto login is enabled but no character is selected!", LogEventLevel.Warning);
            
            return;
        }
        
        characterLobby.Join(character);
    }

    /// <summary>
    /// Auto login: Send login request to the server after the shard list has been received.
    /// </summary>
    /// <param name="shardList"></param>
    private void ShardListOnShardListUpdated(ShardList shardList)
    {
        if (!GameConfig.EnableAutoLogin)
            return;
        
        var selectedAccount = AccountService.GetAccount(GameConfig.AutoLoginId);
        if (selectedAccount == null)
        {
            _ = Kernel.TriggerInterrupt("Auto login is enabled but no account is selected!", LogEventLevel.Warning);
            
            return;
        }
        
        var shard = shardList.Shards.FirstOrDefault(s => string.Equals(s.Name, GameConfig.AutoLoginServer, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(GameConfig.AutoLoginServer) );
        if (shard == null)
        {
            _ = Kernel.TriggerInterrupt("Auto login is enabled but no server is selected or the server could not be found!", LogEventLevel.Warning);
            
            return;
        }
        
        // Bot start -> auto login fail -> shutdown?
        Log.Information("Auto login enabled. Logging in...");
        Login(selectedAccount.Username, selectedAccount.Password, shard);
    }
}