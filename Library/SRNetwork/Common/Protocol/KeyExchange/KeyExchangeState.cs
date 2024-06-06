namespace SRNetwork.Common.Protocol.KeyExchange;

public enum KeyExchangeState : byte
{
    Uninitialized,
    Initialized,
    Challenged,
    Accepted,
}