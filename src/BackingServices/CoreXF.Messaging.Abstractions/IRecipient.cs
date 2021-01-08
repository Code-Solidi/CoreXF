using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions
{
    public interface IRecipient
    {
        string Identity { get; }

        IMessageResponse Recieve(IRequestMessage message);
    }
}