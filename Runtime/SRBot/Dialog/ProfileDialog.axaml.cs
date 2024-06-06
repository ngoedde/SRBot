using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using SRCore;
using SRGame.Client;
using SukiUI.Controls;

namespace SRBot.Dialog;

public partial class ProfileDialog : GlassCard
{
    public ProfileDialog()
    {
        InitializeComponent();
    }

    private async void LoadProfileButtonClicked(object? sender, RoutedEventArgs e)
    {
        SukiHost.CloseDialog();

        if (DataContext! is not ProfileDialogModel model)
            return;

        if (model.SelectedProfile == null)
            return;

        await model.SetActiveProfile(model.SelectedProfile);
        await model.SaveProfiles();
        
        var kernel = App.ServiceProvider.GetRequiredService<Kernel>();
        if (kernel.IsInitialized)
            await kernel.ShutdownAsync();
        
        await kernel.InitializeAsync();
        await kernel.InitializeGameAsync(model.SelectedProfile.ClientDirectory, ClientType.Vietnam188);
    }

    private void CancelButtonClicked(object? sender, RoutedEventArgs e)
    {
        SukiHost.CloseDialog();
    }

    private void NewProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext! is not ProfileDialogModel model)
            return;

        model.AddProfile();
    }

    private async void BrowseButtonClicked(object? sender, RoutedEventArgs e)
    {
        var model = DataContext as ProfileDialogModel;
        var kernel = App.ServiceProvider.GetRequiredService<Kernel>();

        if (model.SelectedProfile == null)
            return;

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

        if (model.SelectedProfile.ClientDirectory != selectedFolder)
        {
            model.SelectedProfile.ClientDirectory = selectedFolder;
        }
    }
}