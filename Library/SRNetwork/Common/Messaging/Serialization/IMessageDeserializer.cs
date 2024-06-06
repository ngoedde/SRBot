namespace SRNetwork.Common.Messaging.Serialization;

public interface IMessageDeserializer
{
    bool TryDeserialize(IMessageReader reader);
}