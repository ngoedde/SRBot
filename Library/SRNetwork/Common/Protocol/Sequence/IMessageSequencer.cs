namespace SRNetwork.Common.Protocol.Sequence;

public interface IMessageSequencer
{
    void Initialize(uint value);

    byte Next();
}