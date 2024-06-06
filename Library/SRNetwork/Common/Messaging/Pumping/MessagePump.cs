﻿using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace SRNetwork.Common.Messaging.Pumping;

public class MessagePump : IMessagePump
{
    private readonly ConcurrentQueue<Message> _queue = new ConcurrentQueue<Message>();

    public void Enqueue(Message message) => _queue.Enqueue(message);

    public bool TryGetMessage([MaybeNullWhen(false)] out Message message) => _queue.TryDequeue(out message);
}