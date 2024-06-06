namespace SRPack;

public interface IAsyncFileAdapter
{
    #region Properties
    
    /// <summary>
    ///     Returns the file name to the file or folder that is being used in the adapter.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    ///     Returns a value indicating if the file adapter is initialized.
    /// </summary>
    public bool IsInitialized { get; }
    
    #endregion

    #region Methods
    
    /// <summary>
    ///     Read all bytes from a file asynchronously.
    /// </summary>
    /// <param name="filePath">The absolute path to the file.</param>
    /// <returns></returns>
    public Task<byte[]> ReadAllBytesAsync(string filePath);

    /// <summary>
    ///     Read all text from a file asynchronously.
    /// </summary>
    /// <param name="filePath">The absolute path to the file.</param>
    /// <returns></returns>
    public Task<string> ReadAllTextAsync(string filePath);

    /// <summary>
    ///     Read all content from a file asynchronously.
    /// </summary>
    /// <param name="filePath">The absolute path to the file.</param>
    /// <returns></returns>
    public Task<MemoryStream> ReadAllAsync(string filePath);

    /// <summary>
    ///     Get all files and folders from a folder asynchronously.
    /// </summary>
    /// <param name="folderPath">The absolute path to the folder.</param>
    /// <returns></returns>
    public Task<IEnumerable<string>> GetFilesAndFoldersAsync(string folderPath);

    /// <summary>
    ///     Get all sub-folders from a folder asynchronously.
    /// </summary>
    /// <param name="folderPath">The absolute path to the folder.</param>
    /// <returns></returns>
    public Task<IEnumerable<string>> GetFoldersAsync(string folderPath);

    /// <summary>
    ///     Get all files from a folder asynchronously.
    /// </summary>
    /// <param name="folderPath">The absolute path to the folder.</param>
    /// <returns></returns>
    public Task<IEnumerable<string>> GetFilesAsync(string folderPath);

    /// <summary>
    ///     Get the entry info of a file or folder asynchronously.
    /// </summary>
    /// <param name="path">The absolute path to the file or folder.</param>
    /// <returns></returns>
    public Task<FileSystemEntryInfo> GetEntryAsync(string path);

    /// <summary>
    ///     Gets all sub-entries of the given folder path.
    /// </summary>
    /// <param name="folderPath">The absolute path to the file or folder.</param>
    /// <returns></returns>
    public Task<IEnumerable<FileSystemEntryInfo>> GetEntriesAsync(string folderPath);

    /// <summary>
    ///     Initializes the file adapter.
    /// </summary>
    /// <returns></returns>
    public Task InitializeAsync();

    /// <summary>
    ///     Closes the file adapter.
    /// </summary>
    /// <returns></returns>
    public Task CloseAsync();

    /// <summary>
    ///     Returns a value indicating if the file or folder exists asynchronously.
    /// </summary>
    /// <param name="filePath">The absolute path to the file or folder</param>
    /// <returns></returns>
    public Task<bool> ExistsAsync(string filePath);
    
    #endregion Methods
}