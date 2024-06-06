namespace SRPack.SRAdapter.Utils;

internal static class PathUtils
{
    private static readonly char SeparatorChar = Path.DirectorySeparatorChar;

    public static string GetParent(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        var index = path.LastIndexOf(SeparatorChar);
        return index == -1 ? string.Empty : path[..index];
    }

    public static string Combine(string pathA, string pathB)
    {
        return Path.Combine(pathA, pathB);
    }

    public static IEnumerable<string> Explode(string path)
    {
        return path.Split(SeparatorChar);
    }

    public static string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    public static string Normalize(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        return path
            .Replace('\\', SeparatorChar)
            .Replace('/', SeparatorChar)
            .TrimStart(SeparatorChar)
            .TrimEnd(SeparatorChar)
            .ToLower();
    }
}