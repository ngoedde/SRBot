using SRNetwork.Common.Messaging;

namespace SRNetwork;

public static class AgentMsgId
{
    public static readonly MessageID
        ReadyToPlay = MessageID.Create(MessageDirection.NoDir, MessageType.Game, 0x12); // 0x34A7
    
    public static readonly MessageID
        LoginActionReq = MessageID.Create(MessageDirection.Req, MessageType.Framework, 0x103); // 0x6103

    public static readonly MessageID
        LoginActionAck = MessageID.Create(MessageDirection.Ack, MessageType.Framework, 0x103); // 0xA103

    public static readonly MessageID
        CharacterListActionReq = MessageID.Create(MessageDirection.Req, MessageType.Game, 0x007); // 0x7007

    public static readonly MessageID
        CharacterListActionResponse = MessageID.Create(MessageDirection.Ack, MessageType.Game, 0x007); // 0xB007

    public static readonly MessageID
        CharacterListJoinReq = MessageID.Create(MessageDirection.Req, MessageType.Game, 0x001); // 0x7008

    public static readonly MessageID
        CharacterListJoinAck = MessageID.Create(MessageDirection.Ack, MessageType.Game, 0x001); // 0xB001

    public static readonly MessageID
        CharacterDataStart = MessageID.Create(MessageDirection.NoDir, MessageType.Game, 0x4A5); // 0x34A5

    public static readonly MessageID
        CharacterDataEnd = MessageID.Create(MessageDirection.NoDir, MessageType.Game, 0x4A6); // 0x34A5

    public static readonly MessageID
        CharacterReset = MessageID.Create(MessageDirection.NoDir, MessageType.Game, 0x4B5); // 0x34A5

    public static readonly MessageID
        CharacterData = MessageID.Create(MessageDirection.NoDir, MessageType.Game, 0x13);
    public static readonly MessageID
        CharacterStatsUpdate = MessageID.Create(MessageDirection.NoDir, MessageType.Game, 0x3D);
    
    public static readonly MessageID
        GroupSpawnStart = MessageID.Create(MessageDirection.NoDir, MessageType.Game, 0x17);
    public static readonly MessageID
        GroupSpawnEnd = MessageID.Create(MessageDirection.NoDir, MessageType.Game, 0x18);
    public static readonly MessageID
        GroupSpawnData = MessageID.Create(MessageDirection.NoDir, MessageType.Game, 0x19);

    public static readonly MessageID
        EntityUpdateAngle = MessageID.Create(MessageDirection.Ack, MessageType.Game, 0x24);

    public static readonly MessageID
        EntityUpdatePosition = MessageID.Create(MessageDirection.Ack, MessageType.Game, 0x23);

    public static readonly MessageID
        EntityUpdateMovement = MessageID.Create(MessageDirection.Ack, MessageType.Game, 0x21);

    public static readonly MessageID
        EntityUpdateSpeed = MessageID.Create(MessageDirection.Ack, MessageType.Game, 0xD0);

}
