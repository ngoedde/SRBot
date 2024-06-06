namespace SRPack;

public class LocalFileAdapter(string basePath) : IAsyncFileAdapter
{
    public string FileName { get; } = basePath;
    public bool IsInitialized { get; private set; }

    public async Task<byte[]> ReadAllBytesAsync(string filePath)
    {
        filePath = GetAbsolutePath(filePath);

        return await File.ReadAllBytesAsync(filePath);
    }

    public async Task<string> ReadAllTextAsync(string filePath)
    {
        filePath = GetAbsolutePath(filePath);

        return await File.ReadAllTextAsync(filePath);
    }

    public async Task<MemoryStream> ReadAllAsync(string filePath)
    {
        filePath = GetAbsolutePath(filePath);
        var buffer = await ReadAllBytesAsync(filePath);

        return new MemoryStream(buffer, false);
    }

    public Task<IEnumerable<string>> GetFilesAndFoldersAsync(string folderPath)
    {
        folderPath = GetAbsolutePath(folderPath);

        return Task.FromResult(Directory.GetFileSystemEntries(folderPath).AsEnumerable());
    }

    public Task<IEnumerable<string>> GetFoldersAsync(string folderPath)
    {
        folderPath = GetAbsolutePath(folderPath);

        return Task.FromResult(Directory.GetDirectories(folderPath).AsEnumerable());
    }

    public Task<IEnumerable<string>> GetFilesAsync(string folderPath)
    {
        folderPath = GetAbsolutePath(folderPath);

        return Task.FromResult(Directory.GetFiles(folderPath).AsEnumerable());
    }

    public Task<FileSystemEntryInfo> GetEntryAsync(string path)
    {
        return Task.FromResult(new FileSystemEntryInfo(path));
    }

    public Task InitializeAsync()
    {
        if (!Directory.Exists(basePath))
        {
            throw new DirectoryNotFoundException($"Directory {basePath} not found");
        }

        IsInitialized = true;

        return Task.CompletedTask;
    }

    public Task CloseAsync()
    {
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string filePath)
    {
        filePath = GetAbsolutePath(filePath);

        return Task.FromResult(File.Exists(filePath) || Directory.Exists(filePath));
    }

    public Task<IEnumerable<FileSystemEntryInfo>> GetEntriesAsync(string folderPath)
    {
        folderPath = GetAbsolutePath(folderPath);

        return Task.FromResult(Directory.GetFileSystemEntries(folderPath).Select(x => new FileSystemEntryInfo(x)));
    }

    private string GetAbsolutePath(string relativePath)
    {
        return Path.Combine(basePath, relativePath);
    }
}