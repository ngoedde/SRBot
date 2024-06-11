﻿using SRNetwork.Common.Messaging;

namespace SRNetwork;

public static class AgentMsgId
{
    public static readonly MessageID
        CharacterListActionReq = MessageID.Create(MessageDirection.Req, MessageType.Game, 0x007); // 0x7007
    public static readonly MessageID
        CharacterListActionResponse = MessageID.Create(MessageDirection.Ack, MessageType.Game, 0x007); // 0xB007
    public static readonly MessageID
        LoginActionReq = MessageID.Create(MessageDirection.Req, MessageType.Framework, 0x103); // 0x6103
    public static readonly MessageID
        LoginActionAck = MessageID.Create(MessageDirection.Ack, MessageType.Framework, 0x103); // 0xA103
    public static readonly MessageID
        CharacterListJoinReq = MessageID.Create(MessageDirection.Req, MessageType.Game, 0x001); // 0x7008
    public static readonly MessageID
        CharacterListJoinAck = MessageID.Create(MessageDirection.Ack, MessageType.Game, 0x001); // 0xB001
}