namespace SRNetwork.Common.Messaging.Serialization.Collection;

public interface IMessageCollectionSerializer
{
    bool Serialize<T>(IMessageWriter writer, IReadOnlyCollection<T> collection)
        where T : IMessageSerializer;
}