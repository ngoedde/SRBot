﻿using System.Diagnostics;
using SRNetwork.Common.Messaging;

namespace SRNetwork.Common.Protocol.Encoding.Encryption;

internal class MessageEncryptor : IMessageEncryptor
{
    public static IMessageEncryptor Shared { get; } = new MessageEncryptor();

    public EncodeResult Encrypt(IMessageEncodingContext session, Message msg)
    {
        if (!msg.Encrypted)
            return EncodeResult.Success;

        if ((session.Options & ProtocolOptions.Encryption) == 0)
        {
            Console.WriteLine($"{nameof(this.Encrypt)}: Context does not support encryption. [{msg.ID}]");
            msg.Encrypted = false;
            return EncodeResult.Success;
            //return EncodeResult.InvalidHeader;
        }

        var inputLength = Message.HEADER_ENC_SIZE + msg.DataSize;
        var outputLength = Blowfish.GetOutputLength(inputLength);
        if (outputLength + Message.HEADER_ENC_OFFSET > Message.BUFFER_SIZE)
        {
            Console.WriteLine($"{nameof(this.Encrypt)}: Message exceeded buffer size. [{msg.ID}]");
            msg.Encrypted = false;
            return EncodeResult.Success;
            //return EncodeResult.InvalidMsgSize;
        }

        var result = session.Blowfish.Encode(msg.GetSpan(Message.HEADER_ENC_OFFSET));
        Debug.Assert(result == outputLength);
        return EncodeResult.Success;
    }
}