namespace SRNetwork.Common.Protocol;

public interface IMessageCryptoContext
{
    ProtocolOptions Options { get; set; }
    Blowfish Blowfish { get; }
}