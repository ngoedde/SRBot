using System.Text.Json;
using Serilog;

namespace SRCore.Config;

public class ConfigService(ILogger logger)
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };
    
    private Dictionary<string, object> _configurations = new();
    
    public async Task<T> LoadConfigurationAsync<T>(string path, T defaultValue)
    {
        if (defaultValue == null) 
            throw new ArgumentNullException(nameof(defaultValue));
        
        if (!File.Exists(path))
        {
            _configurations[path] = defaultValue;
            
            await SaveAsync(path);
            
            return defaultValue;
        }
        
        await using var stream = File.OpenRead(path);
        var config = await JsonSerializer.DeserializeAsync<T>(stream);
        if (config == null)
            return defaultValue;

        RemoveConfigOfType<T>();
        
        _configurations[path] = config;
        
        logger.Information($"Loaded configuration from {path}");
        return config;
    }

    public async Task SaveAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        
        if (!_configurations.TryGetValue(path, out var config))
            return;
        
        var jsonString = JsonSerializer.Serialize(config, _options);

        await File.WriteAllTextAsync(path, jsonString);
        
        logger.Information($"Saved configuration to {path}");
    }
    
    public async Task SaveAllAsync()
    {
        foreach (var config in _configurations.Keys)
        {
            await SaveAsync(config);
        }
    }

    public void Clear()
    {
        _configurations.Clear();
    }

    public T? GetConfig<T>()
    {
        return _configurations.Where(t => t.Value.GetType() == typeof(T)).Select(t => (T)t.Value).FirstOrDefault();
    }
    
    private void RemoveConfigOfType<T>()
    {
        var key = _configurations.FirstOrDefault(t => t.Value.GetType() == typeof(T)).Key;
        if (key != null)
            _configurations.Remove(key);
    }
}