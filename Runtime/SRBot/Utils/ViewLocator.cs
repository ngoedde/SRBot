using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace SRBot.Utils;

public class ViewLocator(AppPluginLoader pluginLoader) : IDataTemplate
{
    private readonly Dictionary<object, Control> _controlCache = new();


    public Control Build(object? data)
    {
        var fullName = data?.GetType().FullName;
        if (fullName is null)
            return new TextBlock { Text = "Data is null or has no name." };

        var name = fullName.Replace("PageModel", "Page");
        name = name.Replace("WindowModel", "Window");
        name = name.Replace("DialogModel", "Dialog");

        var type = pluginLoader.GetTypeFromAssemblies(name);

        if (type is null)
            return new TextBlock { Text = $"No View For {name}." };

        if (!_controlCache.TryGetValue(data!, out var res))
        {
            res ??= (Control)Activator.CreateInstance(type)!;
            _controlCache[data!] = res;
        }

        res.DataContext = data;
        return res;
    }

    public bool Match(object? data) => data is INotifyPropertyChanged;
}