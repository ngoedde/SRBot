using System.Text;
using SRNetwork.Common.Messaging.Serialization;
using SRNetwork.Common.Messaging.Serialization.Collection;

namespace SRNetwork.Common.Messaging;

public interface IMessageWriter
{
    bool TryWrite<T>(T value) where T : unmanaged;

    bool TryWrite<T>(ref T value) where T : unmanaged;

    bool TryWrite(DateTime value);

    bool TryWrite(string? value);

    bool TryWrite(string? value, Encoding encoding);

    bool TryWrite(string? value, int length);

    bool TryWrite(string? value, int length, Encoding encoding);

    bool TryWrite<T>(ReadOnlySpan<T> values) where T : unmanaged;

    bool TrySerialize<T>(in T serializeable) where T : IMessageSerializer;

    bool TrySerialize<T>(IReadOnlyCollection<T> collection, IMessageCollectionSerializer collectionSerializer)
        where T : IMessageSerializer;
}