namespace SRNetwork.Common.Messaging.Serialization.Collection;

public interface IMessageCollectionDeserializer
{
    bool Deserialize<T>(IMessageReader reader, ICollection<T> collection)
        where T : IMessageDeserializer, new();
}