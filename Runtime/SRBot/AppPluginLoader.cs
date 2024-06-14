using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;

namespace SRBot;

public class AppPluginLoader(ILogger logger)
{
    public Dictionary<Assembly, List<AppPlugin>> Plugins { get; private set; } = new();

    public string PluginDirectory => Environment.CurrentDirectory;

    public List<AppPlugin> LoadPlugins()
    {
        //foreach (var file in Directory.GetFiles(PluginDirectory, "*.Plugin.*.dll"))
        //{
        //    if(TryGetExtensionsFromAssembly(file, out var assembly, out var plugins)) {
        //        //Plugins.Add(assembly, plugins);
        //    }
        //}

        return Plugins.SelectMany(p => p.Value).ToList();
    }

    public IEnumerable<AppPlugin> GetPlugins() => Plugins.Values.SelectMany(p => p);
    public IEnumerable<Assembly> GetAssemblies() => Plugins.Keys;

    private bool TryGetExtensionsFromAssembly(string file, out Assembly assembly, out List<AppPlugin> plugins)
    {
        var result = new List<AppPlugin>();

        plugins = new List<AppPlugin>();
        assembly = null!;

        try
        {
            assembly = Assembly.LoadFrom(file);
            var assemblyTypes = assembly.GetTypes();
            foreach (var type in assemblyTypes.Where(t => !t.IsAbstract && t.IsPublic && t.IsClass))
            {
                if (!type.IsSubclassOf(typeof(AppPlugin)))
                    continue;

                if (Activator.CreateInstance(type) is not AppPlugin plugin)
                {
                    logger.Error($"Failed to create plugin instance for [{file}]");

                    continue;
                }

                //Do only support one plugin per assembly.
                result.Add(plugin);

                return true;
            }
        }
        catch
        {
            logger.Warning($"Ignoring invalid extension assembly [{file}]");
            /* ignore, it's an invalid extension */
        }

        return false;
    }

    public Type? GetTypeFromAssemblies(string typeName)
    {
        var type = Assembly.GetExecutingAssembly().GetType(typeName);
        if (type != null)
        {
            return type;
        }

        foreach (var assembly in Plugins.Keys)
        {
            type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }

        return null;
    }
}