using SRNetwork.Common.Protocol.CRC;
using SRNetwork.Common.Protocol.Sequence;

namespace SRNetwork.Common.Protocol;

public interface IMessageEncodingContext : IMessageCryptoContext
{
    bool IsTrusted { get; set; }
    public IMessageChecksummer Checksummer { get; }
    public IMessageSequencer Sequencer { get; }
}