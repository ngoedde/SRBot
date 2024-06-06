namespace SRNetwork.Common.Messaging.Serialization;

public interface IMessageSerializer
{
    bool TrySerialize(IMessageWriter writer);
}