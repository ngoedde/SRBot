using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRGame.Client;
using SRGame.Client.Entity.RefObject;
using SRGame.Client.Repository;
using ViewLocator = SRBot.Utils.ViewLocator;

namespace SRBot.Windows;

public class ObjectBrowserWindowModel(EntityManager entityManager): ViewModel
{
    private string _filter = string.Empty;
    public ICollection<RefObjItem> Items => (ICollection<RefObjItem>)entityManager.ItemRepository.Entities.Values;
    [Reactive] public RefObjItem SelectedItem { get; set; } = entityManager.ItemRepository.Entities.First().Value;

    [Reactive] public ObservableCollection<RefObjItem> ItemsSearchResult { get; set; } = new();
    
    [Reactive] public string ItemsTabTitle { get; set; } = "Items";

    public string Filter
    {
        get => _filter;
        set
        {
            this.RaiseAndSetIfChanged(ref _filter, value);
            
            ItemsSearchResult = SearchItems();
            ItemsTabTitle = $"Items ({ItemsSearchResult.Count})";
        }
    }
    
    public ObservableCollection<RefObjItem> SearchItems()
    {
        if (string.IsNullOrEmpty(_filter))
            return new ObservableCollection<RefObjItem>(Items);
    
        return new ObservableCollection<RefObjItem>(Items.Where(x => 
            x.Name.Contains(_filter) ||
            x.CodeName.Contains(_filter) ||
            x.Id.ToString().Contains(_filter) ||
            x.Tid.ToString().Contains(_filter)
        ));
    }
}