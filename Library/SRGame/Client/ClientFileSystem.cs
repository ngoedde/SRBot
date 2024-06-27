using Serilog;
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

        try
        {
            return assetPack switch
            {
                AssetPack.Media => await Media!.ReadAllTextAsync(path),
                AssetPack.Data => await Data!.ReadAllTextAsync(path),
                _ => throw new ArgumentOutOfRangeException(nameof(assetPack), assetPack, null)
            };
        }
        catch (Exception e)
        {
            Log.Error($"Error loading asset {path}: {e.Message}");

            return string.Empty;
        }
    }

    public async Task<MemoryStream> ReadFileStream(AssetPack assetPack, string path)
    {
        if (!IsInitialized)
            throw new Exception("FileSystem is not initialized!");

        try
        {
            return assetPack switch
            {
                AssetPack.Media => await Media!.ReadAllAsync(path),
                AssetPack.Data => await Data!.ReadAllAsync(path),
                _ => throw new ArgumentOutOfRangeException(nameof(assetPack), assetPack, null)
            };
        }
        catch (Exception e)
        {
            Log.Error($"Error loading asset {path}: {e.Message}");

            return new MemoryStream();
        }
    }
    
    public async Task<byte[]> ReadFile(AssetPack assetPack, string path)
    {
        if (!IsInitialized)
            throw new Exception("FileSystem is not initialized!");

        try
        {
            return assetPack switch
            {
                AssetPack.Media => await Media!.ReadAllBytesAsync(path),
                AssetPack.Data => await Data!.ReadAllBytesAsync(path),
                _ => throw new ArgumentOutOfRangeException(nameof(assetPack), assetPack, null)
            };
        }
        catch (Exception e)
        {
            Log.Error($"Error loading asset {path}: {e.Message}");

            return [];
        }
    }


    public async Task<bool> FileExists(AssetPack assetPack, string path)
    {
        if (!IsInitialized)
            throw new Exception("FileSystem is not initialized!");

        try
        {
            return assetPack switch
            {
                AssetPack.Media => await Media!.ExistsAsync(path),
                AssetPack.Data => await Data!.ExistsAsync(path),
                _ => throw new ArgumentOutOfRangeException(nameof(assetPack), assetPack, null)
            };
        }
        catch (Exception e)
        {
            return false;
        }
    }
}