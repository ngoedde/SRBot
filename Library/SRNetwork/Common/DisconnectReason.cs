namespace SRNetwork.Common;

public enum DisconnectReason
{
    Invalid,
    EngineError,
    EngineShutdown,
    Intentional,
    TimeOut,
    ClosedByPeer,
    ReceiveError,
    SendError,
}