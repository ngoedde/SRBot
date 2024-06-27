using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SRBot.Page.World.Minimap;
using SRCore.Service;

namespace SRBot.Page.World;

public partial class WorldPage : UserControl
{
    private WorldPageModel ViewModel => (WorldPageModel) DataContext;
    private MinimapCanvas _minimapCanvas;
    
    public WorldPage()
    {
        InitializeComponent();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        //Init map canvas
        _minimapCanvas = this.Find<MinimapCanvas>("Minimap");
        _minimapCanvas!.Initialize(ViewModel.ClientFileSystem, ViewModel.Spawn, ViewModel.Player);
 
        ViewModel.MainLoopRegistry.Register(_minimapCanvas.Update);
        //Register update method
        // ViewModel.MainLoopRegistry.Register((delta) =>
        // {
        //     _minimapCanvas.Update(delta);
        // });
    }
}