using System.Text;

namespace SRPack.SRAdapter.Utils;

public static class BinaryReaderExtension
{
    // public static string ReadString(this BinaryReader reader, int length)
    // {
    //     var bytes = reader.ReadBytes(length);
    //     return Encoding.GetEncoding("ISO-8859-1").GetString(bytes).TrimEnd('\0');
    // }
    //
    public static string ReadStringFixed(this BinaryReader reader)
    {
        var byteCount = reader.ReadInt32();

        return reader.ReadString(byteCount);
    }

    public static string ReadString(this BinaryReader reader, int byteCount)
    {
        var buffer = reader.ReadBytes(byteCount);

        var terminatorOffset = byteCount;
        for (var i = 0; i < byteCount; i++)
            if (buffer[i] == byte.MinValue)
            {
                terminatorOffset = i;
                break;
            }

        return Encoding.ASCII.GetString(buffer, 0, terminatorOffset);
    }
}