using Material.Icons;
using ReactiveUI.Fody.Helpers;

namespace SRBot.Page;

public abstract class PageModel(
    string name,
    string title,
    int position = 0,
    MaterialIconKind icon = MaterialIconKind.Home)
    : ViewModel
{
    [Reactive] public string Name { get; set; } = name;
    [Reactive] public MaterialIconKind Icon { get; set; } = icon;
    [Reactive] public string Title { get; set; } = title;
    [Reactive] public int Position { get; set; } = position;
    [Reactive] public bool Visible { get; set; } = true;
}