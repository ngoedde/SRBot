using SRNetwork.Common.Messaging;

namespace SRNetwork.Common.Protocol.Decoding;

public interface IMessageDecoder
{
    public DecodeResult Decode(IMessageEncodingContext context, Message msg);
}