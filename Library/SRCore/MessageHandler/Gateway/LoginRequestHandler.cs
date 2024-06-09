﻿using SRCore.Models;
using SRNetwork.SilkroadSecurityApi;
using SRNetwork;


namespace SRCore.MessageHandler.Gateway
{
    internal class LoginRequestHandler(AgentLogin agentLogin) : SRNetwork.MessageHandler
    {
        public override PacketHandler Handler => Handle;

        public override ushort Opcode => GatewayMsgId.LoginReq;

        public override ValueTask<bool> Handle(Session session, Packet packet)
        {
            try
            {
                agentLogin.ContentId = packet.ReadByte();
                agentLogin.Username = packet.ReadString();
                agentLogin.Password = packet.ReadString();
                agentLogin.ShardId = packet.ReadUShort();

                return ValueTask.FromResult(true);
            }
            catch (Exception e)
            {
                return ValueTask.FromResult(false);
            }
        }
    }
}