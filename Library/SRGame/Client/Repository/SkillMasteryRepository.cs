using SRGame.Client.Entity.RefObject;

namespace SRGame.Client.Repository;

public class SkillMasteryRepository(ClientFileSystem clientFileSystem)
    : EntityRepository<RefSkillMastery, int>(clientFileSystem)
{
    public override async Task LoadAsync(ClientType clientType)
    {
        await base.LoadAsync(clientType);

        var data = await ReadTextFileLines("server_dep/silkroad/textdata/skillmasterydata.txt");
        ParseLinesToEntities(data);

        OnLoaded();
    }
}