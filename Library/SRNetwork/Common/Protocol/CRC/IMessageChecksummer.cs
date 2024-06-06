namespace SRNetwork.Common.Protocol.CRC;

public interface IMessageChecksummer
{
    byte Compute(Span<byte> buffer, int length);

    void Initialize(uint seed);
}