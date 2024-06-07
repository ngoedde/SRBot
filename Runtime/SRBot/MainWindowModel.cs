using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Collections;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRBot.Page;
using SRCore;
using SRCore.Config;
using SRCore.Config.Model;
namespace SRBot;

public class MainWindowModel : ViewModel
{
    private readonly ProfileService _profileService;
    private readonly Game _game;

    public IAvaloniaReadOnlyList<PageModel> Pages { get; }
    
    [Reactive] public string Title { get; set; } = "SRBot";

    public Profile ActiveProfile => _profileService.ActiveProfile;

    [Reactive] public bool IsLoading { get; set; }
    [Reactive] public string LoadingText { get; set; }
    
    public bool IsGameInitialized => _game.IsLoaded;

    public MainWindowModel(IEnumerable<PageModel> pages, ProfileService profileService, Game game)
    {
        Pages = new AvaloniaList<PageModel>(pages.OrderBy(x => x.Position));
        
        _profileService = profileService;
        _game = game;

        _profileService.ActiveProfileChanged += ProfileServiceOnActiveProfileChanged;   
        _game.GameStopLoading += OnGameStopLoading;
        _game.GameStartLoading += OnGameStartLoading;
    }

    private void OnGameStopLoading(Game kernel)
    {
        SetLoading(false);
    }

    private void OnGameStartLoading(Game kernel)
    {
        SetLoading(true, "Loading game...");
    }

    public void SetLoading(bool loading, string text = "")
    {
        IsLoading = loading;
        LoadingText = text;
        
        this.RaisePropertyChanged(nameof(IsLoading));
        this.RaisePropertyChanged(nameof(LoadingText));
        this.RaisePropertyChanged(nameof(IsGameInitialized));
    }
    
    private void ProfileServiceOnActiveProfileChanged(Profile profile)
    {
        this.RaisePropertyChanged(nameof(ActiveProfile));
    }
}