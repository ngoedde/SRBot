namespace SRNetwork.Common.Protocol.KeyExchange;

public enum KeyExchangeResult : byte
{
    InvalidState,
    Success,
    InvalidMsg,
    InvalidSignature
}