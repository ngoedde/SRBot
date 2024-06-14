﻿using System.Diagnostics;
using System.Threading.Channels;
using SRNetwork.Common.Extensions;
using SRNetwork.Common.Messaging.Handling;

namespace SRNetwork.Common.Messaging.Posting;

public class AsyncMessagePoster : IAsyncMessagePoster
{
    private readonly int _id;
    private readonly ChannelWriter<Message> _localWriter;
    private readonly AsyncMsgHandler<Message> _handleMsg;
    private readonly AsyncMsgHandler<Message> _sendMsg;

    public AsyncMessagePoster(int id, ChannelWriter<Message> localWriter, AsyncMsgHandler<Message> handleMsg,
        AsyncMsgHandler<Message> sendMsg)
    {
        _id = id;
        _localWriter = localWriter;
        _handleMsg = handleMsg;
        _sendMsg = sendMsg;
    }

    public ValueTask<bool> PostMsgAsync(Message msg, CancellationToken cancellationToken = default)
    {
        if (msg.ID == MessageID.Empty)
        {
            Console.WriteLine($"{nameof(this.PostMsgAsync)}: Invalid ID");
            return ValueTask.FromResult(false);
        }

        const int MSG_TARGET_INVALID = -1;
        if (msg.ReceiverID == MSG_TARGET_INVALID)
        {
            Console.WriteLine($"{nameof(this.PostMsgAsync)}: Invalid ReceiverID");
            return ValueTask.FromResult(false);
        }

        if (msg.SenderID == MSG_TARGET_INVALID)
        {
            Console.WriteLine($"{nameof(this.PostMsgAsync)}: Invalid SenderID");
            return ValueTask.FromResult(false);
        }

        // Local
        if (msg.ReceiverID == _id && msg.SenderID == _id)
        {
            msg.Retain();
            return _localWriter.TryWriteAsync(msg, cancellationToken);
        }

        if (msg.ReceiverID == _id && msg.SenderID != _id)
            return _handleMsg(msg, cancellationToken);
        else if (msg.ReceiverID != _id && msg.SenderID == _id)
            return _sendMsg(msg, cancellationToken);

        return ValueTask.FromException<bool>(new UnreachableException());
    }
}