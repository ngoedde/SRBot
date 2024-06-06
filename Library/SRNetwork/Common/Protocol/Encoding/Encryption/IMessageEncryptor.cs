using SRNetwork.Common.Messaging;

namespace SRNetwork.Common.Protocol.Encoding.Encryption;

public interface IMessageEncryptor
{
    EncodeResult Encrypt(IMessageEncodingContext session, Message msg);
}