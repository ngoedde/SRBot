using SRGame.Client.Cryptography;
using SRGame.Client.Entity.RefObject;

namespace SRGame.Client.Repository;

public class SkillRepository(ClientFileSystem clientFileSystem) : EntityRepository<RefSkill, int>(clientFileSystem)
{
    public override async Task<EntityRepository<RefSkill, int>> LoadAsync(ClientType clientType)
    {
        await base.LoadAsync(clientType);

        var listFilePath = "server_dep/silkroad/textdata/skilldataenc.txt";
        if (clientType > ClientType.Vietnam274)
            listFilePath = "server_dep/silkroad/textdata/skilldata.txt";

        var listFileContent = await ReadTextFileLines(listFilePath);
        var skillDecrypter = new SkillCryptographyProvider();

        foreach (var dataFile in listFileContent)
        {
            if (string.IsNullOrEmpty(dataFile))
                continue;

            var fileContent = await FileSystem.ReadFileBytes(AssetPack.Media, $"server_dep/silkroad/textdata/{dataFile}");
            IEnumerable<string> lines;
            if (clientType <= ClientType.Vietnam274)
            {
                skillDecrypter.DecryptStream(fileContent, out var decodedStream);
                var reader = new StreamReader(decodedStream);

                lines = (await reader.ReadToEndAsync()).Split('\n').Select(f => f.Trim('\r'));
            }
            else
                lines = await this.ReadTextFileLines($"server_dep/silkroad/textdata/{dataFile}");

            foreach (var itemLine in lines)
            {
                if (string.IsNullOrEmpty(itemLine))
                    continue;

                var parser = new EntityParser(itemLine);
                if (!parser.TryParse<RefSkill, int>(out var item))
                    continue;

                Entities[item.Id] = item;
            }
        }

        OnLoaded();

        return this;
    }
}