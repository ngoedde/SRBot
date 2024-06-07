using Serilog;
using SRCore.Config;
using SRGame.Client;

namespace SRCore;

public class Game(EntityManager entityManager, ClientInfoManager clientInfoManager, ClientFileSystem fileSystem, ProfileService profileService)
{
    public delegate void GameInitializedEventHandler(Game game);
    public event GameInitializedEventHandler? GameInitialized;

    public delegate void GameStartLoadingEventHandler(Game game);
    public event GameStartLoadingEventHandler? GameStartLoading;
    
    public delegate void GameStopLoadingEventHandler(Game game);
    public event GameStopLoadingEventHandler? GameStopLoading;
    
    public bool IsLoaded { get; private set; }

    public async Task LoadGameDataAsync()
    {
        IsLoaded = false;
        
        OnGameStartLoading(this);
        
        if (!Directory.Exists(profileService.ActiveProfile.ClientDirectory))
        {
            Log.Warning("Silkroad directory not set! Game can not be initialized.");

            return;
        }
        
        //Init file system
        if (!fileSystem.IsInitialized)
        {
            var mediaFile = Directory.GetFiles(profileService.ActiveProfile.ClientDirectory, "media.pk2", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
            var dataFile = Directory.GetFiles(profileService.ActiveProfile.ClientDirectory, "data.pk2", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            if (mediaFile == null || dataFile == null)
            {
                throw new Exception("Game asset packs (media.pk2, data.pk2) not found!");
            }

            await fileSystem.InitializeAsync(
                mediaFile,
                dataFile
            );
        }

        await clientInfoManager.LoadAsync().ConfigureAwait(false);
        await entityManager.LoadAsync(profileService.ActiveProfile.ClientType).ConfigureAwait(false);

        IsLoaded = true;
        
        OnGameStopLoading(this);
        OnGameInitialized(this);
    }

    public async Task CloseAsync()
    {
        IsLoaded = false;
        entityManager.Clear();

        if (!fileSystem.IsInitialized) return;
        
        await fileSystem.Media?.CloseAsync();
        await fileSystem.Data?.CloseAsync();
    }

    protected virtual void OnGameInitialized(Game game)
    {
        GameInitialized?.Invoke(game);
    }

    protected virtual void OnGameStartLoading(Game game)
    {
        GameStartLoading?.Invoke(game);
    }

    protected virtual void OnGameStopLoading(Game game)
    {
        GameStopLoading?.Invoke(game);
    }
}