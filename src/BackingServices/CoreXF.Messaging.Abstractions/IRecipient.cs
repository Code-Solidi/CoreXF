using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions
{
    public interface IRecipient
    {
        string Identity { get; set; }

        IMessageResponse Recieve(IRequestResponseMessage message);
    }
}