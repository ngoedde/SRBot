namespace SRNetwork.Common.Messaging.Posting;

public interface IAsyncMessagePoster
{
    public ValueTask<bool> SendMsgAsync(int receiverID, Message msg)
    {
        msg.ReceiverID = receiverID;
        return this.PostMsgAsync(msg);
    }

    public ValueTask<bool> PostMsgAsync(Message msg, CancellationToken cancellationToken = default);
}
