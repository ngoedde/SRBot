using ReactiveUI.Fody.Helpers;
using Serilog;
using SRCore.Config;
using SRGame.Client;

namespace SRCore;

public class Game
{
    private readonly EntityManager _entityManager;
    private readonly ClientInfoManager _clientInfoManager;
    private readonly ClientFileSystem _fileSystem;
    private readonly ProfileService _profileService;

    public Game(
        EntityManager entityManager,
        ClientInfoManager clientInfoManager,
        ClientFileSystem fileSystem,
        ProfileService profileService
    )
    {
        _entityManager = entityManager;
        _clientInfoManager = clientInfoManager;
        _fileSystem = fileSystem;
        _profileService = profileService;
    }

    public delegate void GameInitializedEventHandler(Game game);

    public event GameInitializedEventHandler? GameInitialized;

    public delegate void GameStartLoadingEventHandler(Game game);

    public event GameStartLoadingEventHandler? GameStartLoading;

    public delegate void GameStopLoadingEventHandler(Game game);

    public event GameStopLoadingEventHandler? GameStopLoading;

    [Reactive] public bool IsLoaded { get; private set; }

    public async Task LoadGameDataAsync()
    {
        IsLoaded = false;

        OnGameStartLoading(this);

        if (!Directory.Exists(_profileService.ActiveProfile.ClientDirectory))
        {
            Log.Warning("Silkroad directory not set! Game can not be initialized.");

            return;
        }

        try
        {
            //Init file system
            if (!_fileSystem.IsInitialized)
            {
                var mediaFile = Directory.GetFiles(_profileService.ActiveProfile.ClientDirectory, "media.pk2",
                        SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();
                var dataFile = Directory.GetFiles(_profileService.ActiveProfile.ClientDirectory, "data.pk2",
                        SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();

                if (mediaFile == null || dataFile == null)
                    throw new Exception("Game asset packs (media.pk2, data.pk2) not found!");

                await _fileSystem.InitializeAsync(
                    mediaFile,
                    dataFile
                );
            }

            await _clientInfoManager.LoadAsync().ConfigureAwait(false);
            await _entityManager.LoadAsync(_profileService.ActiveProfile.ClientType).ConfigureAwait(false);

            IsLoaded = true;
        }
        catch (Exception e)
        {
            Log.Error($"Failed to load game data: {e.Message}");
        }

        OnGameStopLoading(this);
        OnGameInitialized(this);
    }

    public async Task Close()
    {
        IsLoaded = false;
        _entityManager.Clear();

        if (!_fileSystem.IsInitialized) return;

       await _fileSystem.Media?.CloseAsync(); 
       await _fileSystem.Data?.CloseAsync();
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