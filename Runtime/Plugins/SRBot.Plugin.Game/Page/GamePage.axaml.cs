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

    private async void ChangeDirectory_OnClick(object? sender, RoutedEventArgs e)
    {
        var kernel = App.ServiceProvider.GetRequiredService<Kernel>();

        if (kernel.IsGameInitialized)
        {
            var msgBoxDialogModel = new MessageBoxDialogModel()
            {
                Icon = MaterialIconKind.MessageWarning,
                IconColor = Brushes.Gold,
                Title = "The game is already initialized.",
                Message =
                    "The game is already initialized. If you choose a different Silkroad Online client directory, the bot will reload all data. Do you want to continue?",
            };
            
            SukiHost.ShowDialog(msgBoxDialogModel);
            
            if (await msgBoxDialogModel.WaitForResultAsync() != UserConfirmation.Ok)
                return;
        }

        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);
        var selectedFolders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Select the Silkroad Online game directory",
        });

        var selectedFolder = selectedFolders.FirstOrDefault()?.Path.AbsolutePath;
        if (selectedFolder == null)
            return;

        var model = DataContext as GamePageModel;

        if (model!.ActiveProfile?.ClientDirectory != selectedFolder)
        {
            model.ActiveProfile.ClientDirectory = selectedFolder;

            await kernel.InitializeGameAsync(model.ActiveProfile.ClientDirectory, ClientType.Vietnam188);
        }
    }
}