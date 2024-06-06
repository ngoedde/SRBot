using SRNetwork.Common.Messaging;

namespace SRNetwork.Common.Protocol.Decoding.Decryption;

public interface IMessageDecryptor
{
    DecodeResult Decrypt(IMessageEncodingContext context, Message msg);
}