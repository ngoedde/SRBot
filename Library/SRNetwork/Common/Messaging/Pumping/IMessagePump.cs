using System.Diagnostics.CodeAnalysis;

namespace SRNetwork.Common.Messaging.Pumping;

public interface IMessagePump
{
    void Enqueue(Message message);

    bool TryGetMessage([MaybeNullWhen(false)] out Message message);
}