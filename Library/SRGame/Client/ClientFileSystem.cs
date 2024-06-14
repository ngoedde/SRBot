using SRPack;

namespace SRGame.Client;

public class ClientFileSystem
{
    public bool IsInitialized => Media is { IsInitialized: true } && Data is { IsInitialized: true };

    public IAsyncFileAdapter? Media { get; private set; }
    public IAsyncFileAdapter? Data { get; private set; }

    public string Path => System.IO.Path.GetDirectoryName(Media?.FileName) ?? string.Empty;

    public async Task InitializeAsync(string mediaPath, string dataPath)
    {
        Media = new SRFileAdapter(mediaPath);
        Data = new SRFileAdapter(dataPath);

        await Media.InitializeAsync();
        await Data.InitializeAsync();
    }

    public async Task<string> ReadFileText(AssetPack assetPack, string path)
    {
        if (!IsInitialized)
            throw new Exception("FileSystem is not initialized!");

        return assetPack switch
        {
            AssetPack.Media => await Media!.ReadAllTextAsync(path),
            AssetPack.Data => await Data!.ReadAllTextAsync(path),
            _ => throw new ArgumentOutOfRangeException(nameof(assetPack), assetPack, null)
        };
    }

    public async Task<MemoryStream> ReadFileBytes(AssetPack assetPack, string path)
    {
        if (!IsInitialized)
            throw new Exception("FileSystem is not initialized!");

        return assetPack switch
        {
            AssetPack.Media => await Media!.ReadAllAsync(path).ConfigureAwait(false),
            AssetPack.Data => await Data!.ReadAllAsync(path).ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(assetPack), assetPack, null)
        };
    }

    public async Task<bool> FileExists(AssetPack assetPack, string path)
    {
        if (!IsInitialized)
            throw new Exception("FileSystem is not initialized!");

        return assetPack switch
        {
            AssetPack.Media => await Media!.ExistsAsync(path),
            AssetPack.Data => await Data!.ExistsAsync(path),
            _ => throw new ArgumentOutOfRangeException(nameof(assetPack), assetPack, null)
        };
    }
}