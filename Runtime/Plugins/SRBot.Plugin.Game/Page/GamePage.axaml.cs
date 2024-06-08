using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SRBot.Dialog;
using SRCore;
using SRGame;
using SRGame.Client;
using SukiUI.Controls;

namespace SRBot.Plugin.Game.Page;

public partial class GamePage : UserControl
{
    public GamePage()
    {
        InitializeComponent();
    }
}