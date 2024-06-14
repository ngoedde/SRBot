using System.Diagnostics;
using System.Text;
using Serilog;
using SRGame.Client.Entity.RefObject;
using SRGame.Client.Repository;

namespace SRGame.Client;

public sealed class EntityManager(
    ClientFileSystem fileSystem,
    ILogger logger)
{
    public delegate void OnStartLoadingEventHandler();

    public event OnStartLoadingEventHandler? StartLoading;

    public delegate void OnFinishLoadingEventHandler();

    public event OnFinishLoadingEventHandler? FinishLoading;

    public ItemRepository ItemRepository { get; } = new(fileSystem);

    public SkillRepository SkillRepository { get; } = new(fileSystem);

    public CharacterRepository CharacterRepository { get; } = new(fileSystem);

    public TranslationRepository TranslationRepository { get; } = new(fileSystem);

    public SkillMasteryRepository SkillMasteryRepository { get; } = new(fileSystem);

    public bool IsLoading { get; private set; }

    public async Task LoadAsync(ClientType clientType)
    {
        IsLoading = true;
        OnStartLoading();

        var sw = Stopwatch.StartNew();

        try
        {
            await ItemRepository.LoadAsync(clientType);
            await SkillRepository.LoadAsync(clientType);
            await CharacterRepository.LoadAsync(clientType);
            await TranslationRepository.LoadAsync(clientType);
            await SkillMasteryRepository.LoadAsync(clientType);
        }
        catch (Exception e)
        {
            logger.Fatal(e, $"Game initialization error: {e.Message}");
        }

        IsLoading = false;
        logger.Information("Game loaded in {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
        logger.Information(PrintStats());

        OnFinishLoading();
    }

    public string GetText(string name) => !TranslationRepository.Entities.TryGetValue(name, out var translation)
        ? name
        : translation.Text;

    public RefText? GetTranslation(string name) => TranslationRepository.GetEntity(name);

    public RefObjItem? GetItem(string codeName) =>
        ItemRepository.Entities.FirstOrDefault(i => i.Value.CodeName == codeName).Value;

    public RefObjItem? GetItem(int id) => ItemRepository.GetEntity(id);

    public RefSkill? GetSkill(string codeName) =>
        SkillRepository.Entities.FirstOrDefault(s => s.Value.CodeName == codeName).Value;

    public RefSkill? GetSkill(int id) => SkillRepository.GetEntity(id);

    public RefObjChar? GetCharacter(string codeName) =>
        CharacterRepository.Entities.FirstOrDefault(s => s.Value.CodeName == codeName).Value;

    public RefObjChar? GetCharacter(int id) => CharacterRepository.GetEntity(id);

    private string PrintStats()
    {
        var sb = new StringBuilder();
        sb.AppendLine("");
        sb.AppendLine("ItemRepository:");
        sb.AppendLine($"  Loaded {ItemRepository.Entities.Count} items");
        sb.AppendLine("SkillRepository:");
        sb.AppendLine($"  Loaded {SkillRepository.Entities.Count} skills");
        sb.AppendLine("CharacterRepository:");
        sb.AppendLine($"  Loaded {CharacterRepository.Entities.Count} characters");
        sb.AppendLine("TranslationRepository:");
        sb.AppendLine($"  Loaded {TranslationRepository.Entities.Count} translations");
        sb.AppendLine("SkillMasteryRepository:");
        sb.AppendLine($"  Loaded {SkillMasteryRepository.Entities.Count} skill masteries");

        return sb.ToString();
    }

    private void OnStartLoading() => StartLoading?.Invoke();

    private void OnFinishLoading() => FinishLoading?.Invoke();

    public void Clear()
    {
        ItemRepository.Clear();
        SkillRepository.Clear();
        CharacterRepository.Clear();
        TranslationRepository.Clear();
        SkillMasteryRepository.Clear();
    }
}