namespace SRNetwork.Common.Messaging;

[Flags]
public enum MessageDirection : byte
{
    NoDir = 0,
    Req = 1,
    Ack = 2,
}