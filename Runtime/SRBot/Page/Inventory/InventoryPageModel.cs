using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using SRBot.Utils;
using SRCore.Models;
using SRCore.Models.Inventory;
using SRGame.Client;

namespace SRBot.Page.Inventory;

public class InventoryPageModel : PageModel
{
    private readonly IServiceProvider _serviceProvider;
    public Player Player => _serviceProvider.GetRequiredService<Player>();

    public ObservableCollection<InventoryItemModel> PlayerInventory { get; } = new();

    private IconCache IconCache => _serviceProvider.GetRequiredService<IconCache>();

    public InventoryPageModel(IServiceProvider serviceProvider) : base("srbot_page_inventory", "Inventory", 3,
        MaterialIconKind.BagPersonal)
    {
        _serviceProvider = serviceProvider;

        Player.Inventory.CollectionChanged += PlayerInventoryOnCollectionChanged;
    }

    private async void PlayerInventoryOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var item in e.NewItems.Cast<Item>())
            {
                PlayerInventory.Add(new InventoryItemModel(item, IconCache));
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var item in e.OldItems.Cast<Item>())
            {
                var model = PlayerInventory.FirstOrDefault(x => x.Item == item);
                if (model != null)
                {
                    PlayerInventory.Remove(model);
                }
            }
        }
    }
}