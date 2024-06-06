using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRBot.Page;
using SRCore;
using SRCore.Config;
using SRCore.Config.Model;
using SRGame.Client;

namespace SRBot;

public class MainWindowModel : ViewModel
{
    private readonly ProfileService? _profileService;
    private readonly EntityManager _entityManager;
    private readonly Kernel _kernel;

    public IAvaloniaReadOnlyList<PageModel> Pages { get; }
    
    [Reactive] public string Title { get; set; } = "SRBot";

    [Reactive] public Profile? ActiveProfile => _profileService?.ActiveProfile;

    [Reactive] public bool IsLoading { get; set; }
    [Reactive] public string LoadingText { get; set; }
    
    public bool IsGameInitialized => _kernel.IsGameInitialized;

    public MainWindowModel(IEnumerable<PageModel> pages, ProfileService profileService, EntityManager entityManager, Kernel kernel)
    {
        Pages = new AvaloniaList<PageModel>(pages.OrderBy(x => x.Position));
        
        _profileService = profileService;
        _entityManager = entityManager;
        _kernel = kernel;
        
        _profileService.ActiveProfileChanged += ProfileServiceOnActiveProfileChanged;   
        _kernel.StartLoading += KernelOnStartLoading;
        _kernel.StopLoading += KernelOnStopLoading;
    }

    private void KernelOnStopLoading(Kernel kernel)
    {
        SetLoading(false);
    }

    private void KernelOnStartLoading(Kernel kernel)
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