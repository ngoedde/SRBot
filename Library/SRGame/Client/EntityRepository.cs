using System.Collections.Concurrent;
using SRGame.Client.Entity;
using SRGame.Client.Repository;

namespace SRGame.Client;

public abstract class EntityRepository<TEntity, TPrimaryKey>(ClientFileSystem fileSystem)
    where TEntity : Entity<TPrimaryKey>
    where TPrimaryKey : notnull
{
    protected ClientFileSystem FileSystem => fileSystem;

    public delegate void LoadedEventArgs(EntityRepository<TEntity, TPrimaryKey> sender);

    public event LoadedEventArgs? Loaded;

    public ConcurrentDictionary<TPrimaryKey, TEntity> Entities { get; } = new();
    
    public TEntity? GetEntity(TPrimaryKey id) => Entities.GetValueOrDefault(id);
    
    public ClientType ClientType { get; private set; } = ClientType.Vietnam188;

    public virtual Task<EntityRepository<TEntity, TPrimaryKey>> LoadAsync(ClientType clientType)
    {
        ClientType = clientType;

        if (!fileSystem.IsInitialized)
            throw new Exception("FileSystem is not initialized");

        Entities.Clear();

        return Task.FromResult(this);
    }

    protected virtual void OnLoaded()
    {
        Loaded?.Invoke(this);
    }

    public void Translate(TranslationRepository translationRepository)
    {
        // Cache the properties
        var entityType = typeof(TEntity);
        var translationProperties = entityType.GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(TranslationAttribute)))
            .ToDictionary(p => p, p => entityType.GetProperty(((TranslationAttribute)p.GetCustomAttributes(typeof(TranslationAttribute), false).FirstOrDefault()).FieldName));
        
        foreach (var entity in Entities.Values)
        {
            foreach (var entry in translationProperties)
            {
                var property = entry.Key;
                var translationProperty = entry.Value;

                if (translationProperty == null)
                    throw new Exception($"Translation field for property {property.Name} not found");

                var translation = translationRepository.GetEntity(translationProperty.GetValue(entity) as string);
                if (translation == null)
                    continue;

                property.SetValue(entity, translation.Text);
            }
        }
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