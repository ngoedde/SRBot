using SRGame.Client.Entity.RefObject;

namespace SRGame.Client.Repository;

public class CharacterRepository(ClientFileSystem clientFileSystem)
    : EntityRepository<RefObjChar, int>(clientFileSystem)
{
    public override async Task LoadAsync(ClientType clientType)
    {
        await base.LoadAsync(clientType);

        var listFileContent = await this.ReadTextFileLines("server_dep/silkroad/textdata/characterdata.txt");

        foreach (var dataFile in listFileContent)
        {
            if (string.IsNullOrEmpty(dataFile))
                continue;

            var data = await ReadTextFileLines($"server_dep/silkroad/textdata/{dataFile}");

            ParseLinesToEntities(data);
        }

        OnLoaded();
    }
}