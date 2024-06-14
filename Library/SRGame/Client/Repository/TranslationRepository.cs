using SRGame.Client.Entity.RefObject;

namespace SRGame.Client.Repository;

public class TranslationRepository(ClientFileSystem clientFileSystem)
    : EntityRepository<RefText, string>(clientFileSystem)
{
    public override async Task<EntityRepository<RefText, string>> LoadAsync(ClientType clientType)
    {
        await base.LoadAsync(clientType);

        var files = new List<string>();
        if (clientType >= ClientType.Global)
        {
            files.AddRange(
                (await ReadTextFileLines("server_dep/silkroad/textdata/textuisystem.txt")).Select(f =>
                    $"server_dep/silkroad/textdata/{f}"));
            files.AddRange(
                (await ReadTextFileLines("server_dep/silkroad/textdata/textzonename.txt")).Select(f =>
                    $"server_dep/silkroad/textdata/{f}"));
            files.AddRange(
                (await ReadTextFileLines("server_dep/silkroad/textdata/textdata_object.txt")).Select(f =>
                    $"server_dep/silkroad/textdata/{f}"));
            files.AddRange(
                (await ReadTextFileLines("server_dep/silkroad/textdata/textdata_equip&skill.txt")).Select(f =>
                    $"server_dep/silkroad/textdata/{f}"));
            files.AddRange(
                (await ReadTextFileLines("server_dep/silkroad/textdata/textquest_speech&name.txt")).Select(f =>
                    $"server_dep/silkroad/textdata/{f}"));
            files.AddRange(
                (await ReadTextFileLines("server_dep/silkroad/textdata/textquest_otherstring.txt")).Select(f =>
                    $"server_dep/silkroad/textdata/{f}"));
            files.AddRange(
                (await ReadTextFileLines("server_dep/silkroad/textdata/textquest_queststring.txt")).Select(f =>
                    $"server_dep/silkroad/textdata/{f}"));
        }
        else
        {
            files.Add("server_dep/silkroad/textdata/textuisystem.txt");
            files.Add("server_dep/silkroad/textdata/textzonename.txt");

            files.AddRange(
                (await ReadTextFileLines("server_dep/silkroad/textdata/textdataname.txt")).Select(f =>
                    $"server_dep/silkroad/textdata/{f}"));
            files.AddRange(
                (await ReadTextFileLines("server_dep/silkroad/textdata/textquest.txt")).Select(f =>
                    $"server_dep/silkroad/textdata/{f}"));
        }

        foreach (var file in files)
        {
            var data = await ReadTextFileLines(file);

            ParseLinesToEntities(data);
        }

        OnLoaded();
        
        return this;
    }

    public string GetTranslation(string name, string? @default = null)
    {
        return Entities.GetValueOrDefault(name)?.Text ?? @default ?? name;
    }
}