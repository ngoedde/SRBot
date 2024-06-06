using SRNetwork.Common.Messaging;

namespace SRNetwork.Common.Protocol.Encoding;

public interface IMessageEncoder
{
    public EncodeResult Encode(IMessageEncodingContext context, Message msg);
}