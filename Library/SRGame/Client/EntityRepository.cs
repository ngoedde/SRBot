using System.Collections.Concurrent;

namespace SRGame.Client;

public abstract class EntityRepository<TEntity, TPrimaryKey>(ClientFileSystem fileSystem)
    where TEntity : Entity.Entity<TPrimaryKey>
    where TPrimaryKey : notnull
{
    protected ClientFileSystem FileSystem => fileSystem;

    public delegate void LoadedEventArgs(EntityRepository<TEntity, TPrimaryKey> sender);

    public event LoadedEventArgs? Loaded;

    public ConcurrentDictionary<TPrimaryKey, TEntity> Entities { get; } = new();
    public TEntity? GetEntity(TPrimaryKey id) => Entities.GetValueOrDefault(id);
    public ClientType ClientType { get; private set; } = ClientType.Vietnam188;

    public virtual Task LoadAsync(ClientType clientType)
    {
        ClientType = clientType;

        if (!fileSystem.IsInitialized)
            throw new Exception("FileSystem is not initialized");

        Entities.Clear();

        return Task.CompletedTask;
    }

    protected virtual void OnLoaded()
    {
        Loaded?.Invoke(this);
    }

    protected async Task<string[]> ReadTextFileLines(string fileName)
    {
        var listFileContent = await FileSystem.ReadFileText(AssetPack.Media, fileName);

        return listFileContent.Split('\n').Select(f => f.Trim('\r')).Where(f => !string.IsNullOrEmpty(f)).ToArray();
    }

    protected virtual void ParseLinesToEntities(string[] data)
    {
        foreach (var line in data)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            var parser = new EntityParser(line);
            if (!parser.TryParse<TEntity, TPrimaryKey>(out var item))
                continue;

            Entities[item.Id] = item;
        }
    }

    public void Clear()
    {
        Entities.Clear();
    }
}