using System;

namespace SRBot.Page;

public class PageNavigationService
{
    public Action<Type>? NavigationRequested { get; set; }

    public void RequestNavigation<T>() where T : PageModel
    {
        NavigationRequested?.Invoke(typeof(T));
    }
}